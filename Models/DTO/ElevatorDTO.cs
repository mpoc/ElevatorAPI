using System.Text.Json;
using System.Text.Json.Serialization;
using ElevatorAPI.Models;

namespace ElevatorAPI.Models.DTO
{
    public class ElevatorDTO
    {
        public int ElevatorId { get; set; }
        public DoorStatus DoorStatus { get; set; }
        public ElevatorStatus ElevatorStatus { get; set; }
        public int AtFloor { get; set; }
        public bool Busy { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BuildingId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BuildingDTO Building { get; set; }

        public ElevatorDTO(Elevator elevator, bool includeBuilding)
        {
            ElevatorId = elevator.ElevatorId;
            DoorStatus = elevator.DoorStatus;
            ElevatorStatus = elevator.ElevatorStatus;
            AtFloor = elevator.AtFloor;
            Busy = elevator.Busy;
            BuildingId = includeBuilding ? elevator.BuildingId : null;
            Building = includeBuilding ? new BuildingDTO(elevator.Building, false) : null;
        }
    }
}
