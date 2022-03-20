using System;
using Microsoft.EntityFrameworkCore;
using NationsAPI.Models;
using Microsoft.Extensions.Logging;
#nullable disable

namespace NationsAPI.Database
{
    public partial class NationsDbContext : DbContext
    {

        public static readonly ILoggerFactory MyLoggerFactory
    = LoggerFactory.Create(builder => { builder.AddConsole(); }); 
        public NationsDbContext() { }

        public NationsDbContext(DbContextOptions<NationsDbContext> options)
            : base(options)
        {
        }

        internal object Include(Func<object, object> p)
        {
            throw new NotImplementedException();
        }

        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Player> Players { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseLoggerFactory(MyLoggerFactory)
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(Environment.GetEnvironmentVariable("NationsAppCon"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Polish_CI_AS");

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message");

                entity.Property(e => e.Content)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.HasOne(d => d.FromPlayer)
                    .WithMany(p => p.MessageFromPlayers)
                    .HasForeignKey(d => d.FromPlayerId)
                    .HasConstraintName("FK__Message__FromPlayer__2180FB33");

                entity.HasOne(d => d.ToPlayer)
                    .WithMany(p => p.MessageToPlayers)
                    .HasForeignKey(d => d.ToPlayerId)
                    .HasConstraintName("FK__Message__ToPlayer__208CD6FA");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("Player");

                entity.Property(e => e.Nick)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });
            modelBuilder.Seed();
            OnModelCreatingPartial(modelBuilder);

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
