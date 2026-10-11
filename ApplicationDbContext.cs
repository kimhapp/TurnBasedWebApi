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
            modelBuilder.Entity<Player>(entity =>
            {
                entity.Property(p => p.Name).HasMaxLength(20);
                entity.Property(p => p.Email).HasMaxLength(255);
                entity.HasIndex(p => p.Email).IsUnique();
                entity.Property(p => p.HashedPassword).HasMaxLength(255);
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
                entity.Property(p => p.UpdatedAt).HasDefaultValueSql("now()");
                
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Player_Email_Format",
                    "\"Email\" ~ '^[^@]+@[^@]+$'"
                ));
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.Property(m => m.Code).HasMaxLength(16);
                entity.HasIndex(m => m.Code).IsUnique().HasFilter("\"StartedAt\" IS NULL");
                entity.Property(m => m.CreatedAt).HasDefaultValueSql("now()");
                entity.Property(m => m.Version).IsRowVersion();
 
                entity.HasOne<Player>().WithMany().HasForeignKey(m => m.CurrentHostPlayerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<Player>().WithMany().HasForeignKey(m => m.CurrentTurnPlayerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<Player>().WithMany().HasForeignKey(m => m.WinnerId).OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Match_OverMeansAlreadyStarted",
                        "\"OverAt\" IS NULL OR \"StartedAt\" IS NOT NULL"
                    );

                    t.HasCheckConstraint(
                        "CK_Match_OverMustBeAfterStarted",
                        "\"OverAt\" IS NULL OR \"OverAt\" >= \"StartedAt\""
                    );

                    t.HasCheckConstraint(
                        "CK_Match_WinnerMeansAlreadyOver",
                        "\"WinnerId\" IS NULL OR \"OverAt\" IS NOT NULL"
                    );
                    
                    t.HasCheckConstraint(
                        "CK_Match_OverMeansWinnerExists",
                        "\"OverAt\" IS NULL OR \"WinnerId\" IS NOT NULL"
                    );
                });
            });

            modelBuilder.Entity<MatchPlayer>(entity =>
            {
                entity.HasKey(mp => new { mp.MatchId, mp.PlayerId });
                entity.HasIndex(mp => new { mp.MatchId, mp.Slot }).IsUnique();
                entity.Property(mp => mp.Role).HasConversion<string>().HasMaxLength(20);
                entity.Property(mp => mp.JoinedAt).HasDefaultValueSql("now()");

                entity.HasOne(mp => mp.Match).WithMany(m => m.Players).HasForeignKey(mp => mp.MatchId);
                entity.HasOne(mp => mp.Player).WithMany().HasForeignKey(mp => mp.PlayerId).OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_MatchPlayer_Slot_Range",
                        "\"Slot\" >= 1 AND \"Slot\" <= 2");

                    t.HasCheckConstraint("CK_MatchPlayer_Hp_NonNegative",
                        "\"Hp\" >= 0");
                });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}