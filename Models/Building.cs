using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ElevatorAPI.Models
{
    public class Building
    {
        [Key]
        public int BuildingId { get; set; }
        public int Height { get; set; }

        public List<Elevator> Elevators { get; set; }
    }
}
