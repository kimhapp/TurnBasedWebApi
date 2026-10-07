using Microsoft.EntityFrameworkCore;
using TurnBasedWebApi.Models;

namespace TurnBasedWebApi
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Match> Matches { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<MatchPlayer> MatchPlayers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>().HasIndex(m => m.Code).IsUnique();
            modelBuilder.Entity<MatchPlayer>().HasIndex(mp => new { mp.MatchId, mp.Slot }).IsUnique();
            modelBuilder.Entity<MatchPlayer>().HasIndex(mp => new { mp.MatchId, mp.PlayerId }).IsUnique();
            modelBuilder.Entity<MatchPlayer>().HasOne(mp => mp.Match).WithMany(m => m.Players).HasForeignKey(mp => mp.MatchId);
            modelBuilder.Entity<MatchPlayer>().HasOne(mp => mp.Player).WithMany().HasForeignKey(mp => mp.PlayerId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}