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
    public class BuildingsController : ControllerBase
    {
        private readonly IBuildingRepository _buildingRepository;

        public BuildingsController(IBuildingRepository buildingRepository) {
            _buildingRepository = buildingRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Building>> Get()
        {
            return await _buildingRepository.Get();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Building>> Get(int id)
        {
            return await _buildingRepository.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<Building>> Post([FromBody] Building building)
        {
            var newBuilding = await _buildingRepository.Create(building);
            return CreatedAtAction(nameof(Get), new { id = newBuilding.Id }, newBuilding);
        }

        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromBody] Building building)
        {
            if (id != building.Id)
            {
                return BadRequest();
            }

            await _buildingRepository.Update(building);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var buildingToDelete = await _buildingRepository.Get(id);
            if (buildingToDelete == null)
            {
                return NotFound();
            }

            await _buildingRepository.Delete(buildingToDelete.Id);
            return NoContent();
        }
    }
}
