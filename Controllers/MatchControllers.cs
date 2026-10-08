using Microsoft.AspNetCore.Mvc;
using TurnBasedWebApi.Dtos;
using TurnBasedWebApi.Models;
using TurnBasedWebApi.Services;
using TurnBasedWebApi.Utils;

namespace TurnBasedWebApi.Controllers
{
    // Player Id and Match Id have to be manually input as there's no auth option for now
    [ApiController]
    [Route("/api/[controller]")]
    public class MatchController(IMatchService matchService, IPlayerService playerService) : ControllerBase
    {
        IActionResult MapMatchResult(MatchResult result)
        {
            return result.Status switch
            {
                MatchStatus.Success => result.Match == null ? NoContent() : Ok(result.Match.ToDto()),
                MatchStatus.NotFound => NotFound(result.Error),
                MatchStatus.NotAllowed => StatusCode(StatusCodes.Status403Forbidden, result.Error),
                MatchStatus.Conflict => Conflict(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpPost]
        public async Task<IActionResult> CreateMatch(CreateMatchDto createMatchDto)
        {
            PlayerResult playerResult = await playerService.CreatePlayerAsync(createMatchDto.Name);
            if (playerResult.Status != PlayerStatus.Success) return BadRequest(playerResult.Error); 
            
            Player player = playerResult.Player!;

            MatchResult matchResult = await matchService.CreateMatchAsync(player.Id, createMatchDto.Code, createMatchDto.Role);
            return MapMatchResult(matchResult);
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinMatch(JoinMatchDto joinMatchDto)
        {
            PlayerResult playerResult = await playerService.CreatePlayerAsync(joinMatchDto.Name);
            if (playerResult.Status != PlayerStatus.Success) return BadRequest(playerResult.Error); 
            
            Player player = playerResult.Player!;

            MatchResult matchResult = await matchService.JoinMatchAsync(player.Id, joinMatchDto.Code, joinMatchDto.Role);
            return MapMatchResult(matchResult);
        }

        [HttpPost("{matchId}/ready")]
        public async Task<IActionResult> SetReady(Guid matchId, SetReadyDto setReadyDto)
        {
            MatchResult matchResult = await matchService.SetReadyAsync(matchId, setReadyDto.PlayerId, setReadyDto.IsReady);
            return MapMatchResult(matchResult);
        }

        [HttpPost("{matchId}/leave")]
        public async Task<IActionResult> LeaveMatch(Guid matchId, GenericMatchDto genericMatchDto)
        {
            MatchResult matchResult = await matchService.LeaveMatchAsync(matchId, genericMatchDto.PlayerId);
            return MapMatchResult(matchResult);
        }

        [HttpPost("{matchId}/start")]
        public async Task<IActionResult> StartMatch(Guid matchId, GenericMatchDto genericMatchDto)
        {
            MatchResult matchResult = await matchService.StartMatchAsync(matchId, genericMatchDto.PlayerId);
            return MapMatchResult(matchResult);
        }

        [HttpPost("{matchId}/perform")]
        public async Task<IActionResult> PerformAction(Guid matchId, GenericMatchDto genericMatchDto)
        {
            MatchResult matchResult = await matchService.PerformActionAsync(matchId, genericMatchDto.PlayerId);
            return MapMatchResult(matchResult);
        }

        [HttpPost("{matchId}/surrender")]
        public async Task<IActionResult> SurrenderMatch(Guid matchId, GenericMatchDto genericMatchDto)
        {
            MatchResult matchResult = await matchService.SurrenderMatchAsync(matchId, genericMatchDto.PlayerId);
            return MapMatchResult(matchResult);
        }
    }
}