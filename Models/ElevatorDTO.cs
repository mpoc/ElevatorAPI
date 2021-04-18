namespace ElevatorAPI.Models
{
    public class ElevatorDTO
    {
        public DoorStatus DoorStatus { get; set; }
        public ElevatorStatus ElevatorStatus { get; set; }
        public int AtFloor { get; set; }
    }
}
