using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurnBasedWebApi.Dtos;
using TurnBasedWebApi.Services;
using TurnBasedWebApi.Utils;

namespace TurnBasedWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class MatchController(IMatchService matchService) : ControllerBase
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

        Guid GetPlayerGuid() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> CreateMatch(CreateMatchDto createMatchDto)
        {
            MatchResult matchResult = await matchService.CreateMatchAsync(GetPlayerGuid(), createMatchDto.Code, createMatchDto.Role);
            return MapMatchResult(matchResult);
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinMatch(JoinMatchDto joinMatchDto)
        {
            MatchResult matchResult = await matchService.JoinMatchAsync(GetPlayerGuid(), joinMatchDto.Code, joinMatchDto.Role);
            return MapMatchResult(matchResult);
        }

        [HttpPost("{matchId}/ready")]
        public async Task<IActionResult> SetReady(Guid matchId, SetReadyDto setReadyDto)
        {
            MatchResult matchResult = await matchService.SetReadyAsync(matchId, GetPlayerGuid(), setReadyDto.IsReady);
            return MapMatchResult(matchResult);
        }

        [HttpPost("{matchId}/leave")]
        public async Task<IActionResult> LeaveMatch(Guid matchId)
        {
            MatchResult matchResult = await matchService.LeaveMatchAsync(matchId, GetPlayerGuid());
            return MapMatchResult(matchResult);
        }

        [HttpPost("{matchId}/start")]
        public async Task<IActionResult> StartMatch(Guid matchId)
        {
            MatchResult matchResult = await matchService.StartMatchAsync(matchId, GetPlayerGuid());
            return MapMatchResult(matchResult);
        }

        [HttpPost("{matchId}/perform")]
        public async Task<IActionResult> PerformAction(Guid matchId)
        {
            MatchResult matchResult = await matchService.PerformActionAsync(matchId, GetPlayerGuid());
            return MapMatchResult(matchResult);
        }

        [HttpPost("{matchId}/surrender")]
        public async Task<IActionResult> SurrenderMatch(Guid matchId)
        {
            MatchResult matchResult = await matchService.SurrenderMatchAsync(matchId, GetPlayerGuid());
            return MapMatchResult(matchResult);
        }
    }
}