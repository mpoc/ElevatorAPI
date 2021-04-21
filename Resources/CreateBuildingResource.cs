using System.ComponentModel.DataAnnotations;

namespace ElevatorAPI.Resources
{
public class CreateBuildingResource
    {
        [Required]
        public int Height { get; set; }
    }
}
