using TurnBasedWebApi.Models;

namespace TurnBasedWebApi.Dtos
{
    public class MatchPlayerDto
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = null!;
        public Role Role { get; set; }
        public int Slot { get; set; }
        public int Hp { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}