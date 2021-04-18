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
    }
}