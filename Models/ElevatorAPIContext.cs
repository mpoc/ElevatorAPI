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
    }
}