using Microsoft.EntityFrameworkCore;

namespace ElevatorAPI.Models
{
    public class ElevatorContext : DbContext
    {
        public ElevatorContext(DbContextOptions<ElevatorContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Elevator> Elevators { get; set; }
    }
}