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
            modelBuilder.Entity<Player>().Property(p => p.Name).HasMaxLength(20).IsRequired();

            modelBuilder.Entity<Match>().Property(m => m.Code).HasMaxLength(16).IsRequired();
            modelBuilder.Entity<Match>().HasIndex(m => m.Code).IsUnique().HasFilter("\"StartedAt\" IS NULL");
            modelBuilder.Entity<Match>().Property(m => m.Version).IsRowVersion();

            modelBuilder.Entity<MatchPlayer>().HasKey(mp => new { mp.MatchId, mp.PlayerId });
            modelBuilder.Entity<MatchPlayer>().HasIndex(mp => new { mp.MatchId, mp.Slot }).IsUnique();
            modelBuilder.Entity<MatchPlayer>().HasOne(mp => mp.Match).WithMany(m => m.Players).HasForeignKey(mp => mp.MatchId);
            modelBuilder.Entity<MatchPlayer>().HasOne(mp => mp.Player).WithMany().HasForeignKey(mp => mp.PlayerId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}