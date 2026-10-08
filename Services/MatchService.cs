using Microsoft.EntityFrameworkCore;
using TurnBasedWebApi.Models;

namespace TurnBasedWebApi.Services
{
    public interface IMatchService
    {
        Task<Match?> CreateMatchAsync(Guid playerId, string? code, Role role);
        Task<bool> JoinMatchAsync(Guid playerId, string code, Role role);
        Task<bool> SetReadyAsync(Guid matchId, Guid playerId, bool ready);
        Task<bool> LeaveMatchAsync(Guid matchId, Guid playerId);
        Task<Match?> StartMatchAsync(Guid matchId, Guid playerId);
        Task<Match?> PerformActionAsync(Guid matchId, Guid playerId);
        Task<bool> SurrenderMatchAsync(Guid matchId, Guid playerId);
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

        public async Task<Match?> CreateMatchAsync(Guid playerId, string? code, Role role)
        {
            bool isInMatch = await context.MatchPlayers
                .AnyAsync(mp => mp.PlayerId == playerId && !mp.Match.IsOver);
            if (isInMatch) return null;

            Match newMatch = new()
            {
                Code = code ?? GenerateShortCode(7),
                CurrentHostPlayerId = playerId,
                CreatedAt = DateTime.UtcNow,
            };

            newMatch.Players.Add(new MatchPlayer
            {
                PlayerId = playerId,
                IsReady = false,
                Role = role,
                Slot = 1,
                Hp = RoleStats.StartingHp(role),
                JoinedAt = DateTime.UtcNow
            });

            context.Matches.Add(newMatch);
            await context.SaveChangesAsync();
            return newMatch;
        }

        public async Task<bool> JoinMatchAsync(Guid playerId, string code, Role role)
        {
            bool isInMatch = await context.MatchPlayers.AnyAsync(mp => mp.PlayerId == playerId && !mp.Match.IsOver);
            if (isInMatch) return false;

            Match? existingMatch = await context.Matches
                .Include(m => m.Players).FirstOrDefaultAsync(m => m.Code == code && !m.HasStarted);
            if (existingMatch == null) return false;
            if (existingMatch.Players.Count >= MAX_PLAYERS_PER_MATCH) return false;
            
            int nextSlot = Enumerable.Range(1, MAX_PLAYERS_PER_MATCH)
                .First(s => existingMatch.Players.All(p => p.Slot != s));
            existingMatch.Players.Add(new MatchPlayer
            {
                PlayerId = playerId,
                IsReady = false,
                Role = role,
                Slot = nextSlot,
                Hp = RoleStats.StartingHp(role),
                JoinedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetReadyAsync(Guid matchId, Guid playerId, bool ready)
        {
            MatchPlayer? matchPlayer = await context.MatchPlayers
                .FirstOrDefaultAsync(mp => mp.PlayerId == playerId && mp.MatchId == matchId && !mp.Match.HasStarted);
            if (matchPlayer == null) return false;

            matchPlayer.IsReady = ready;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LeaveMatchAsync(Guid matchId, Guid playerId)
        {
            MatchPlayer? matchPlayer = await context.MatchPlayers
                .FirstOrDefaultAsync(mp => mp.PlayerId == playerId && mp.MatchId == matchId && !mp.Match.HasStarted);
            if (matchPlayer == null) return false;

            context.MatchPlayers.Remove(matchPlayer);

            int remaining = await context.MatchPlayers
                .CountAsync(mp => mp.MatchId == matchId && mp.PlayerId != playerId);

            Match? existingMatch = await context.Matches.Include(m => m.Players).FirstAsync(m => m.Id == matchId);
            if (remaining < MIN_PLAYERS_TO_KEEP_MATCH)
            {
                context.Matches.Remove(existingMatch);
            } else if (existingMatch.CurrentHostPlayerId == playerId)
            {
                existingMatch.CurrentHostPlayerId = existingMatch.Players.First(p => p.PlayerId != playerId).PlayerId;
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Match?> StartMatchAsync(Guid matchId, Guid playerId)
        {
            Match? existingMatch = await context.Matches
                .Include(m => m.Players).FirstOrDefaultAsync(m => m.Id == matchId && !m.HasStarted);
            if (existingMatch == null) return null;
            if (existingMatch.CurrentHostPlayerId != playerId) return null;
            if (existingMatch.Players.Count != MAX_PLAYERS_PER_MATCH ||
                !existingMatch.Players.All(p => p.IsReady)) return null;

            existingMatch.CurrentTurnMatchPlayerId = existingMatch.CurrentHostPlayerId;
            existingMatch.StartedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return existingMatch;
        }

        public async Task<Match?> PerformActionAsync(Guid matchId, Guid playerId)
        {
            Match? existingMatch = await context.Matches
                .Include(m => m.Players).FirstOrDefaultAsync(m => m.Id == matchId && m.HasStarted && !m.IsOver);
            if (existingMatch == null) return null;
            if (!existingMatch.Players.Any(p => p.PlayerId == playerId)) return null;
            if (existingMatch.CurrentTurnMatchPlayerId != playerId) return null;

            // This is just a place holder. There will be an actual game logic class in the future
            MatchPlayer performedPlayer = existingMatch.Players.First(p => p.PlayerId == playerId);
            MatchPlayer targetedPlayer = existingMatch.Players.First(p => p.PlayerId != playerId);
            targetedPlayer.Hp -= RoleStats.Attack(performedPlayer.Role);
            if (targetedPlayer.Hp <= 0)
            {
                existingMatch.OverAt = DateTime.UtcNow;
                existingMatch.WinnerId = performedPlayer.PlayerId;
            }
            else
            {
                existingMatch.CurrentTurnMatchPlayerId = targetedPlayer.PlayerId;
            }

            await context.SaveChangesAsync();
            return existingMatch;
        }

        public async Task<bool> SurrenderMatchAsync(Guid matchId, Guid playerId)
        {
            Match? existingMatch = await context.Matches
                .Include(m => m.Players).FirstOrDefaultAsync(m => m.Id == matchId && m.HasStarted && !m.IsOver);
            if (existingMatch == null) return false;
            if (!existingMatch.Players.Any(p => p.PlayerId == playerId)) return false;

            MatchPlayer targetedPlayer = existingMatch.Players.First(p => p.PlayerId != playerId);
            existingMatch.OverAt = DateTime.UtcNow;
            existingMatch.WinnerId = targetedPlayer.PlayerId;

            await context.SaveChangesAsync();
            return true;
        }
    }
}