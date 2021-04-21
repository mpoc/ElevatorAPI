using System.ComponentModel.DataAnnotations;

namespace ElevatorAPI.Resources
{
    public class CreateBuildingWithElevatorsResource
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int NumberOfElevators { get; set; }

        [Required]
        public CreateBuildingResource Building { get; set; }
    }
}
