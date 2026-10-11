using TurnBasedWebApi.Models;

namespace TurnBasedWebApi.Utils
{
    public enum PlayerStatus
    {
        Success,
        NotFound,
        BadRequest,
        Conflict
    }

    public record PlayerResult(PlayerStatus Status, string? Error = null, Player? Player = null)
    {
        public static PlayerResult Success(Player? player = null) => new(PlayerStatus.Success, Player: player);
        public static PlayerResult NotFound(string? error = null) => new(PlayerStatus.NotFound, Error: error);
        public static PlayerResult BadRequest(string? error = null) => new(PlayerStatus.BadRequest, Error: error);
        public static PlayerResult Conflict(string? error = null) => new(PlayerStatus.Conflict, Error: error);
    }
}