using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using ElevatorAPI.Repositories;
using ElevatorAPI.Models;
using ElevatorAPI.Models.DTO;
using ElevatorAPI.Resources;

namespace ElevatorAPI.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly ILogRepository _logRepository;

        public LogsController(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        [SwaggerOperation(Summary = "Get all logs OR get all logs for a specific elevator id")]
        [HttpGet]
        public async Task<IEnumerable<LogDTO>> Get([FromQuery] int? elevatorId)
        {
            var logs = elevatorId.HasValue
                ? await _logRepository.GetByElevator((int)elevatorId)
                : await _logRepository.GetWithElevators();

            return logs.Select(log => new LogDTO(log, true));
        }
    }
}
