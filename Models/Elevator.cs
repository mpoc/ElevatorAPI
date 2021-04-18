namespace ElevatorAPI.Models
{
    public class Elevator
    {
        public int Id { get; set; }
        public DoorStatus DoorStatus { get; set; }
        public ElevatorStatus ElevatorStatus { get; set; }
        public int AtFloor { get; set; }
    }
}
