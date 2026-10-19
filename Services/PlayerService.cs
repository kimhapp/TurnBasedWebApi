using Microsoft.EntityFrameworkCore;
using TurnBasedWebApi.Models;
using TurnBasedWebApi.Utils;

namespace TurnBasedWebApi.Services
{
    public interface IPlayerService
    {
        Task<PlayerResult> RegisterPlayerAsync(string email, string name, string password);
        Task<PlayerResult> LoginPlayerAsync(string name, string password);
    }

    public class PlayerService(ApplicationDbContext context ) : IPlayerService
    {
        public async Task<PlayerResult> RegisterPlayerAsync(string email, string name, string password)
        {
            if (string.IsNullOrWhiteSpace(name)) return PlayerResult.BadRequest("Name is required to register.");
            if (string.IsNullOrWhiteSpace(email)) return PlayerResult.BadRequest("Email is required to register.");
            if (string.IsNullOrWhiteSpace(password)) return PlayerResult.BadRequest("Password is required to register.");

            if (await context.Players.AnyAsync(p => p.Email == email)) return PlayerResult.Conflict("Email has already been registered.");

            Player player = new()
            {
                Name = name,
                Email = email,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };

            context.Players.Add(player);
            await context.SaveChangesAsync();       
            return PlayerResult.Success(player);
        }

        public async Task<PlayerResult> LoginPlayerAsync(string name, string password)
        {
            if (string.IsNullOrWhiteSpace(name)) return PlayerResult.BadRequest("Name is required to login.");
            if (string.IsNullOrWhiteSpace(password)) return PlayerResult.BadRequest("Password is required to login.");

            Player? player = await context.Players.AsNoTracking().FirstOrDefaultAsync(p => p.Name == name);
            if (player == null) return PlayerResult.NotFound("Name does not exist.");

            if (!BCrypt.Net.BCrypt.Verify(password, player.HashedPassword)) return PlayerResult.Conflict("Wrong password.");
     
            return PlayerResult.Success(player);
        }
    }
}