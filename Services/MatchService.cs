using Microsoft.EntityFrameworkCore;
using TurnBasedWebApi.Models;
using TurnBasedWebApi.Utils;

namespace TurnBasedWebApi.Services
{
    public interface IMatchService
    {
        Task<MatchResult> CreateMatchAsync(Guid playerId, string? code, Role role);
        Task<MatchResult> JoinMatchAsync(Guid playerId, string code, Role role);
        Task<MatchResult> SetReadyAsync(Guid matchId, Guid playerId, bool ready);
        Task<MatchResult> LeaveMatchAsync(Guid matchId, Guid playerId);
        Task<MatchResult> StartMatchAsync(Guid matchId, Guid playerId);
        Task<MatchResult> PerformActionAsync(Guid matchId, Guid playerId);
        Task<MatchResult> SurrenderMatchAsync(Guid matchId, Guid playerId);
    }

    public class MatchService(ApplicationDbContext context) : IMatchService
    {
        const int MIN_PLAYERS_TO_KEEP_MATCH = 1;
        const int MAX_PLAYERS_PER_MATCH = 2;

        static string GenerateShortCode(int length)
        {
            const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            char[] chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = alphabet[Random.Shared.Next(alphabet.Length)];
            }
            
            return new string(chars);
        }

        public async Task<MatchResult> CreateMatchAsync(Guid playerId, string? code, Role role)
        {
            Player? player = await context.Players.FindAsync(playerId);
            if (player == null) return MatchResult.NotFound("Player doesn't exist.");

            bool isInMatch = await context.MatchPlayers
                .AnyAsync(mp => mp.PlayerId == playerId && mp.Match.OverAt == null);
            if (isInMatch) return MatchResult.Conflict("Player is already in another match.");

            Match newMatch = new()
            {
                Code = code ?? GenerateShortCode(7),
                CurrentHostPlayerId = playerId,
                CreatedAt = DateTime.UtcNow,
            };

            newMatch.Players.Add(new MatchPlayer
            {
                PlayerId = playerId,
                Player = player,
                IsReady = false,
                Role = role,
                Slot = 1,
                Hp = RoleStats.StartingHp(role),
                JoinedAt = DateTime.UtcNow
            });

            context.Matches.Add(newMatch);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException) when (code == null)
            {
                return MatchResult.Conflict("Could not generate unique code. Please try again!");
            }
            catch (DbUpdateException)
            {
                return MatchResult.Conflict("Match code already in use.");
            }

            return MatchResult.Success(newMatch);
        }

        public async Task<MatchResult> JoinMatchAsync(Guid playerId, string code, Role role)
        {
            Player? player = await context.Players.FindAsync(playerId);
            if (player == null) return MatchResult.NotFound("Player doesn't exist.");
            
            bool isInMatch = await context.MatchPlayers
                .AnyAsync(mp => mp.PlayerId == playerId && mp.Match.OverAt == null);
            if (isInMatch) return MatchResult.Conflict("Player is already in another match.");

            Match? existingMatch = await context.Matches
                .Include(m => m.Players)
                .ThenInclude(mp => mp.Player)
                .FirstOrDefaultAsync(m => m.Code == code && m.StartedAt == null);

            if (existingMatch == null) return MatchResult.NotFound("Match does not exist or has already started.");
            if (existingMatch.Players.Count >= MAX_PLAYERS_PER_MATCH) return MatchResult.Conflict("Match is full.");
            
            int nextSlot = Enumerable.Range(1, MAX_PLAYERS_PER_MATCH)
                .FirstOrDefault(s => existingMatch.Players
                .All(p => p.Slot != s));

            existingMatch.Players.Add(new MatchPlayer
            {
                PlayerId = playerId,
                Player = player,
                IsReady = false,
                Role = role,
                Slot = nextSlot,
                Hp = RoleStats.StartingHp(role),
                JoinedAt = DateTime.UtcNow
            });

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return MatchResult.Conflict("Match is full or player already joined.");
            }

            return MatchResult.Success(existingMatch);
        }

        public async Task<MatchResult> SetReadyAsync(Guid matchId, Guid playerId, bool ready)
        {
            MatchPlayer? matchPlayer = await context.MatchPlayers
                .Include(mp => mp.Match)
                .ThenInclude(m => m.Players)
                .ThenInclude(mp => mp.Player)
                .FirstOrDefaultAsync(mp => mp.PlayerId == playerId && mp.MatchId == matchId && mp.Match.StartedAt == null);

            if (matchPlayer == null) return MatchResult.NotFound("Player is not in this match or match has already started.");

            if (matchPlayer.IsReady != ready)
            {
                matchPlayer.IsReady = ready;
                await context.SaveChangesAsync();
            }

            return MatchResult.Success(matchPlayer.Match);
        }

        public async Task<MatchResult> LeaveMatchAsync(Guid matchId, Guid playerId)
        {
            MatchPlayer? matchPlayer = await context.MatchPlayers
                .FirstOrDefaultAsync(mp => mp.PlayerId == playerId && mp.MatchId == matchId && mp.Match.StartedAt == null);
            if (matchPlayer == null) return MatchResult.NotFound("Player is not in this match or match has already started.");

            context.MatchPlayers.Remove(matchPlayer);

            Match? existingMatch = await context.Matches
                .Include(m => m.Players)
                .FirstOrDefaultAsync(m => m.Id == matchId);
            if (existingMatch == null) return MatchResult.NotFound("Match no longer exists.");

            int remaining = existingMatch.Players.Count(p => p.PlayerId != playerId);

            if (remaining < MIN_PLAYERS_TO_KEEP_MATCH)
            {
                context.Matches.Remove(existingMatch);
            } else if (existingMatch.CurrentHostPlayerId == playerId)
            {
                existingMatch.CurrentHostPlayerId = existingMatch.Players.First(p => p.PlayerId != playerId).PlayerId;
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return MatchResult.Conflict("Match state changed. Please refresh!");
            }

            return MatchResult.Success();
        }

        public async Task<MatchResult> StartMatchAsync(Guid matchId, Guid playerId)
        {
            Match? existingMatch = await context.Matches
                .Include(m => m.Players)
                .ThenInclude(mp => mp.Player)
                .FirstOrDefaultAsync(m => m.Id == matchId && m.StartedAt == null);
            if (existingMatch == null) return MatchResult.NotFound("Match does not exist or has already started.");

            if (existingMatch.CurrentHostPlayerId != playerId) return MatchResult.NotAllowed("Player is not the host.");
            if (existingMatch.Players.Count != MAX_PLAYERS_PER_MATCH ||
                !existingMatch.Players.All(p => p.IsReady)) return MatchResult.Conflict("Match is not full or all players are not ready.");

            existingMatch.CurrentTurnPlayerId = existingMatch.CurrentHostPlayerId;
            existingMatch.StartedAt = DateTime.UtcNow;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return MatchResult.Conflict("Match state changed. Please refresh!");
            }
            
            return MatchResult.Success(existingMatch);
        }

        public async Task<MatchResult> PerformActionAsync(Guid matchId, Guid playerId)
        {
            Match? existingMatch = await context.Matches
                .Include(m => m.Players)
                .ThenInclude(mp => mp.Player)
                .FirstOrDefaultAsync(m => m.Id == matchId && m.StartedAt != null && m.OverAt == null);
            if (existingMatch == null) return MatchResult.NotFound("Match does not exist or hasn't started or is already over.");

            MatchPlayer? performedPlayer = existingMatch.Players.FirstOrDefault(p => p.PlayerId == playerId);
            if (performedPlayer == null) return MatchResult.NotAllowed("Player is not in this match.");
            if (existingMatch.CurrentTurnPlayerId != playerId) return MatchResult.NotAllowed("It is not the player's turn.");

            // This is just a place holder. There will be an actual game logic class in the future
            MatchPlayer targetedPlayer = existingMatch.Players.First(p => p.PlayerId != playerId);
            targetedPlayer.Hp -= RoleStats.Attack(performedPlayer.Role);
            if (targetedPlayer.Hp <= 0)
            {
                existingMatch.OverAt = DateTime.UtcNow;
                existingMatch.WinnerId = performedPlayer.PlayerId;
            }
            else
            {
                existingMatch.CurrentTurnPlayerId = targetedPlayer.PlayerId;
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return MatchResult.Conflict("Action already processed. Please refresh!");
            }

            return MatchResult.Success(existingMatch);
        }

        public async Task<MatchResult> SurrenderMatchAsync(Guid matchId, Guid playerId)
        {
            Match? existingMatch = await context.Matches
                .Include(m => m.Players)
                .ThenInclude(mp => mp.Player)
                .FirstOrDefaultAsync(m => m.Id == matchId && m.StartedAt != null && m.OverAt == null);
            if (existingMatch == null) return MatchResult.NotFound("Match does not exist or hasn't started or is already over.");

            MatchPlayer? performedPlayer = existingMatch.Players.FirstOrDefault(p => p.PlayerId == playerId);
            if (performedPlayer == null) return MatchResult.NotAllowed("Player is not in this match.");

            MatchPlayer targetedPlayer = existingMatch.Players.First(p => p.PlayerId != playerId);
            existingMatch.OverAt = DateTime.UtcNow;
            existingMatch.WinnerId = targetedPlayer.PlayerId;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return MatchResult.Conflict("Match state changed. Please refresh!");
            }

            return MatchResult.Success(existingMatch);
        }
    }
}