namespace TurnBasedWebApi.Models
{
    public class MatchPlayer
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Player Player { get; set; } = null!;
        public Guid MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public Role Role { get; set; }
        public int Slot { get; set; }
        public int Hp { get; set; }
        public bool IsReady { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}