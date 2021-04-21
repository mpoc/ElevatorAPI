using Microsoft.EntityFrameworkCore;

namespace ElevatorAPI.Models
{
    public class ElevatorAPIContext : DbContext
    {
        public ElevatorAPIContext(DbContextOptions<ElevatorAPIContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Elevator> Elevators { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Building>()
                .HasMany(b => b.Elevators)
                .WithOne(e => e.Building)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}