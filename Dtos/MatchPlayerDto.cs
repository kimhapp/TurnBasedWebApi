using TurnBasedWebApi.Models;

namespace TurnBasedWebApi.Dtos
{
    public class MatchPlayerDto
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = null!;
        public Role Role { get; set; }
        public int Slot { get; set; }
        public int Hp { get; set; }
        public bool IsReady { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    public static class MatchPlayerExtensions
    {
        public static MatchPlayerDto ToDto(this MatchPlayer matchPlayer)
        {
            return new MatchPlayerDto
            {
                PlayerId = matchPlayer.PlayerId,
                PlayerName = matchPlayer.Player.Name,
                Slot = matchPlayer.Slot,
                Role = matchPlayer.Role,
                Hp = matchPlayer.Hp,
                IsReady = matchPlayer.IsReady,
                JoinedAt = matchPlayer.JoinedAt
            };
        }
    }
}