namespace ElevatorAPI.Models
{
    public enum ElevatorStatus
    {
        Open,
        Closed,
        Opening,
        Closing
    }

    public class Elevator
    {
        public long Id { get; set; }
        public ElevatorStatus Status { get; set; }
    }
}
