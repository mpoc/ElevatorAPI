using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ElevatorAPI.Models
{
    public class Building
    {
        [Key]
        public int Id { get; set; }
        public int Height { get; set; }
    }
}
