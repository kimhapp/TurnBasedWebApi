namespace TurnBasedWebApi.Dtos
{
    public class MatchDto
    {
        public string Code { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }    
        public DateTime? OverAt { get; set; }
        public int? CurrentTurnMatchPlayerSlot { get; set; }
        public List<MatchPlayerDto> Players { get; set; } = [];
    }
}