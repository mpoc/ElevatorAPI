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
    [Route("api/[controller]")]
    public class ElevatorsController : ControllerBase
    {
        private readonly IElevatorRepository _elevatorRepository;

        public ElevatorsController(IElevatorRepository elevatorRepository) {
            _elevatorRepository = elevatorRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Elevator>> GetElevators()
        {
            return await _elevatorRepository.Get();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Elevator>> GetElevators(int id)
        {
            return await _elevatorRepository.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<Elevator>> PostElevators([FromBody] Elevator elevator)
        {
            var newElevator = await _elevatorRepository.Create(elevator);
            return CreatedAtAction(nameof(GetElevators), new { id = newElevator.Id }, newElevator);
        }

        [HttpPut]
        public async Task<ActionResult> PutBooks(int id, [FromBody] Elevator elevator)
        {
            if (id != elevator.Id)
            {
                return BadRequest();
            }

            await _elevatorRepository.Update(elevator);

            return NoContent();
        }

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
