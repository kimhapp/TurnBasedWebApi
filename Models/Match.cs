namespace TurnBasedWebApi.Models
{
    public class Match
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }    
        public DateTime? OverAt { get; set; }

        public List<MatchPlayer> Players { get; set; } = [];
        public Guid CurrentHostPlayerId { get; set; }
        public Guid? CurrentTurnPlayerId { get; set; }
        public Guid? WinnerId { get; set; }
        public uint Version { get; set; }

        public bool IsOver => OverAt.HasValue;
        public bool HasStarted => StartedAt.HasValue;
    }
}