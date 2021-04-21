using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElevatorAPI.Models.DTO
{
    public class LogDTO
    {
        public int LogId { get; set; }
        public DateTime Date { get; set; }
        public string Info { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ElevatorDTO Elevator { get; set; }

        public LogDTO(Log log, bool includeElevator)
        {
            LogId = log.LogId;
            Date = log.Date;
            Info = log.Info;
            Elevator = includeElevator ? new ElevatorDTO(log.Elevator, false) : null;
        }
    }
}
