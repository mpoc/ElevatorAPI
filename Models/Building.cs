using System.Collections.Generic;

namespace ElevatorAPI.Models
{
    public class Building
    {
        public int Id { get; set; }
        public int Height { get; set; }
        public IEnumerable<Elevator> Elevators { get; set; }
    }
}
