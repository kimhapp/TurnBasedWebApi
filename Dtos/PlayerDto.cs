using System.ComponentModel.DataAnnotations;
using TurnBasedWebApi.Models;

namespace TurnBasedWebApi.Dtos
{
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class RegisterPlayerDto
    {
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = "";

        [Required, StringLength(20, MinimumLength = 3)]
        public string Name { get; set; } = "";

        [Required, StringLength(128, MinimumLength = 3)]
        public string Password { get; set; } = "";
    }

    public class LoginPlayerDto
    {
        [Required, StringLength(20, MinimumLength = 3)]
        public string Name { get; set; } = "";

        [Required, StringLength(128, MinimumLength = 3)]
        public string Password { get; set; } = "";
    }

    public static class PlayerExtensions
    {
        public static PlayerDto ToDto(this Player player)
        {
            return new PlayerDto
            {
                Id = player.Id,
                Name = player.Name,
                CreatedAt = player.CreatedAt
            };
        }
    }
}