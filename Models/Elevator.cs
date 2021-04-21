using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElevatorAPI.Models
{
    public class Elevator
    {
        [Key]
        public int ElevatorId { get; set; }
        public DoorStatus DoorStatus { get; set; }
        public ElevatorStatus ElevatorStatus { get; set; }
        public int AtFloor { get; set; }
        public bool Busy { get; set; }
        
        public int BuildingId { get; set; }
        public Building Building { get; set; }

        public Elevator(Building building)
        {
            DoorStatus = DoorStatus.Closed;
            ElevatorStatus = ElevatorStatus.Idle;
            AtFloor = 0;
            Busy = false;
            Building = building;
        }
    }
}
