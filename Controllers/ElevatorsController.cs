using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using ElevatorAPI.Repositories;
using ElevatorAPI.Models;

namespace ElevatorAPI.Controllers
{
    [ApiController]
    [Route("api/elevators")]
    public class ElevatorsController : ControllerBase
    {
        private readonly IElevatorRepository _elevatorRepository;

        public ElevatorsController(IElevatorRepository elevatorRepository) {
            _elevatorRepository = elevatorRepository;
        }

        [SwaggerOperation(Summary = "Get an elevator by id")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Elevator>> Get(int id)
        {
            var elevator = await _elevatorRepository.Get(id);

            if (elevator == null)
            {
                return NotFound();
            }

            return elevator;
        }

        [SwaggerOperation(Summary = "Delete an elevator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var elevatorToDelete = await _elevatorRepository.Get(id);
            if (elevatorToDelete == null)
            {
                return NotFound();
            }

            await _elevatorRepository.Delete(elevatorToDelete.ElevatorId);
            return NoContent();
        }
    }
}
