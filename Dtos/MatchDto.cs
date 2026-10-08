using System.ComponentModel.DataAnnotations;
using TurnBasedWebApi.Models;

namespace TurnBasedWebApi.Dtos
{
    public class MatchDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }    
        public DateTime? OverAt { get; set; }
        public int? CurrentTurnPlayerSlot { get; set; }
        public Guid? WinnerId { get; set; }
        public List<MatchPlayerDto> Players { get; set; } = [];
    }

    public class CreateMatchDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; } = "";

        [Required]
        public Role Role { get; set; }

        [StringLength(16, MinimumLength = 6)]
        public string? Code { get; set; }
    }

    public class JoinMatchDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; } = "";

        [Required]
        public Role Role { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 6)]
        public string Code { get; set; } = "";
    }

    // Player Id and Match Id have to be manually input as there's no auth option for now
    public class SetReadyDto
    {
        [Required]
        public Guid PlayerId { get; set; }

        [Required]
        public bool IsReady { get; set; }
    }

    public class GenericMatchDto
    {
        [Required]
        public Guid PlayerId { get; set; }
    }

    public static class MatchExtensions
    {
        public static MatchDto ToDto(this Match match)
        {
            return new MatchDto
            {
                Id = match.Id,
                Code = match.Code,
                CreatedAt = match.CreatedAt,
                StartedAt = match.StartedAt,
                OverAt = match.OverAt,
                CurrentTurnPlayerSlot = match.Players.FirstOrDefault(p => p.PlayerId == match.CurrentTurnPlayerId)?.Slot,
                WinnerId = match.WinnerId,
                Players =  match.Players.Select(p => p.ToDto()).ToList()
            };
        }
    }
}