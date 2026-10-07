namespace TurnBasedWebApi.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}