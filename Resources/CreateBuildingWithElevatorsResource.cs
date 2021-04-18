using System.ComponentModel.DataAnnotations;
using ElevatorAPI.Models;

namespace ElevatorAPI.Resources
{
    public class CreateBuildingWithElevatorsResource
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int NumberOfElevators { get; set; }

        [Required]
        public BuildingDTO Building { get; set; }
    }
}
