using Microsoft.EntityFrameworkCore;
using TurnBasedWebApi.Models;

namespace TurnBasedWebApi.Services
{
    public interface IMatchService
    {
        Task<Match?> CreateMatchAsync(Guid playerId, string? code, Role role);
        Task<bool> JoinMatchAsync(Guid matchId, Guid playerId);
        Task<bool> SetReadyAsync(Guid matchId, Guid playerId);
        Task<bool> LeaveMatchAsync(Guid matchId, Guid playerId);
        Task<Match> StartMatchAsync(Guid matchId);
        Task<Match> PerformActionAsync(Guid matchId, Guid playerId);
        Task<bool> SurrenderMatchAsync(Guid matchId, Guid playerId);
    }

    public class MatchService(ApplicationDbContext context) : IMatchService
    {
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
            bool isInMatch = await context.MatchPlayers.AnyAsync(mp => mp.PlayerId == playerId && !mp.Match.IsOver);
            if (isInMatch) return null;

            Match newMatch = new()
            {
                Code = code ?? GenerateShortCode(7),
                StartedAt = DateTime.UtcNow,
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

        public Task<bool> JoinMatchAsync(Guid matchId, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public Task<Match> StartMatchAsync(Guid matchId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetReadyAsync(Guid matchId, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LeaveMatchAsync(Guid matchId, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public Task<Match> PerformActionAsync(Guid matchId, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SurrenderMatchAsync(Guid matchId, Guid playerId)
        {
            throw new NotImplementedException();
        }
    }
}