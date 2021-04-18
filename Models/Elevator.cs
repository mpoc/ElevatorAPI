using System.ComponentModel.DataAnnotations;

namespace ElevatorAPI.Models
{
    public class Elevator
    {
        [Key]
        public int Id { get; set; }
        public DoorStatus DoorStatus { get; set; }
        public ElevatorStatus ElevatorStatus { get; set; }
        public Building Building { get; set; }
        public int AtFloor { get; set; }
    }
}
