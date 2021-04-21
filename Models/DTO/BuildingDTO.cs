using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using ElevatorAPI.Models;

namespace ElevatorAPI.Models.DTO
{
    public class BuildingDTO
    {
        public int BuildingId { get; set; }
        public int Height { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ElevatorDTO> Elevators { get; set; }

        public BuildingDTO(Building building, bool includeElevators)
        {
            BuildingId = building.BuildingId;
            Height = building.Height;
            Elevators = includeElevators ? building.Elevators.Select(e => new ElevatorDTO(e, false)).ToList() : null;
        }
    }
}
