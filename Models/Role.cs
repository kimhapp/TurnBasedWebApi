namespace TurnBasedWebApi.Models
{
    public enum Role
    {
        Warrior,
        Ranger
    }

    public static class RoleStats
    {
        public static int StartingHp(this Role role) => role switch
        {
            Role.Warrior => 30,
            Role.Ranger => 20,
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };

        public static int Attack(this Role role) => role switch
        {
            Role.Warrior => 4,
            Role.Ranger => 8,
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };
    }
}