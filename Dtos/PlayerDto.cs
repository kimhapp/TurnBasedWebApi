using System.ComponentModel.DataAnnotations;

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

        [Required, StringLength(8, MinimumLength = 128)]
        public string Password { get; set; } = "";
    }

    public class LoginPlayerDto
    {
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = "";

        [Required, StringLength(8, MinimumLength = 128)]
        public string Password { get; set; } = "";
    }
}