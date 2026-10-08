using TurnBasedWebApi.Models;

namespace TurnBasedWebApi.Utils
{
    public enum MatchStatus
    {
        Success,
        NotFound,
        NotAllowed,
        Conflict
    }

    public record MatchResult(MatchStatus Status, string? Error = null, Match? Match = null)
    {
        public static MatchResult Success(Match? match = null) => new(MatchStatus.Success, Match: match);
        public static MatchResult NotFound(string? error = null) => new(MatchStatus.NotFound, Error: error);
        public static MatchResult NotAllowed(string? error = null) => new(MatchStatus.NotAllowed, Error: error);
        public static MatchResult Conflict(string? error = null) => new(MatchStatus.Conflict, Error: error);
    }
}