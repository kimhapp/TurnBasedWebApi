using TurnBasedWebApi.Models;
using TurnBasedWebApi.Utils;

namespace TurnBasedWebApi.Services
{
    public interface IPlayerService
    {
        Task<PlayerResult> CreatePlayerAsync(string name);
    }

    public class PlayerService(ApplicationDbContext context) : IPlayerService
    {
        public async Task<PlayerResult> CreatePlayerAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return PlayerResult.BadRequest("Name is required to start match.");

            Player player = new()
            {
                Name = name,
                CreatedAt = DateTime.UtcNow
            };

            context.Players.Add(player);
            await context.SaveChangesAsync();       
            return PlayerResult.Success(player);
        }
    }
}