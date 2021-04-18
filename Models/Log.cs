using System;
using System.Collections.Generic;

namespace ElevatorAPI.Models
{
    public class Log
    {
        public int LogId { get; set; }
        public DateTime Date { get; set; }
        public Elevator Elevator { get; set; }
        public string Info { get; set; }
    }
}
