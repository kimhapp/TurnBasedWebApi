using Microsoft.AspNetCore.Mvc;
using TurnBasedWebApi.Dtos;
using TurnBasedWebApi.Services;
using TurnBasedWebApi.Utils;

namespace TurnBasedWebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PlayerController(IPlayerService playerService, ITokenService tokenService) : ControllerBase
    {
        ActionResult MapPlayerResult(PlayerResult result)
        {
            return result.Status switch
            {
                PlayerStatus.Success => result.Player == null ? NoContent() : Ok(result.Player.ToDto()),
                PlayerStatus.NotFound => NotFound(result.Error),
                PlayerStatus.Conflict => Conflict(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginPlayerDto loginPlayerDto)
        {
            PlayerResult? playerResult = await playerService.LoginPlayerAsync(loginPlayerDto.Name, loginPlayerDto.Password); 
            if (playerResult.Player == null) return MapPlayerResult(playerResult);

            return new TokenResponseDto { Token = tokenService.GenerateToken(playerResult.Player!) };
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenResponseDto>> Register(RegisterPlayerDto registerPlayerDto)
        {
            PlayerResult? playerResult = await playerService.RegisterPlayerAsync(registerPlayerDto.Email, registerPlayerDto.Name, registerPlayerDto.Password); 
            if (playerResult.Player == null) return MapPlayerResult(playerResult);

            return new TokenResponseDto { Token = tokenService.GenerateToken(playerResult.Player!) };
        }
    }
}