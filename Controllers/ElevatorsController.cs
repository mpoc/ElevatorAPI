using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            return await _elevatorRepository.Get(id);
        }

        [SwaggerOperation(Summary = "Create an elevator")]
        [HttpPost]
        public async Task<ActionResult<Elevator>> Post([FromBody] Elevator elevator)
        {
            var newElevator = await _elevatorRepository.Create(elevator);
            return CreatedAtAction(nameof(Get), new { id = newElevator.Id }, newElevator);
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

            await _elevatorRepository.Delete(elevatorToDelete.Id);
            return NoContent();
        }
    }
}
