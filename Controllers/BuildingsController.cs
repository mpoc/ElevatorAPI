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
    [Route("api/buildings")]
    public class BuildingsController : ControllerBase
    {
        private readonly IBuildingRepository _buildingRepository;

        public BuildingsController(IBuildingRepository buildingRepository) {
            _buildingRepository = buildingRepository;
        }

        [SwaggerOperation(Summary = "Get all buildings")]
        [HttpGet]
        public async Task<IEnumerable<BuildingDTO>> Get()
        {
            var buildings = await _buildingRepository.GetWithElevators();
            return buildings.Select(b => new BuildingDTO(b, true));
        }

        [SwaggerOperation(Summary = "Get a building by id")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BuildingDTO>> Get(int id)
        {
            var building = await _buildingRepository.GetWithElevators(id);

            if (building == null)
            {
                return NotFound();
            }
            
            return new BuildingDTO(building, true);
        }

        [SwaggerOperation(Summary = "Create a building with a certain number of elevators")]
        [HttpPost]
        public async Task<ActionResult<Building>> CreateWithElevators([FromBody] CreateBuildingWithElevatorsResource resource)
        {
            if (!ModelState.IsValid)
		        return BadRequest();

            var newBuilding = await _buildingRepository.CreateWithElevators(resource);
            return CreatedAtAction(nameof(CreateWithElevators), new { id = newBuilding.BuildingId }, newBuilding);
        }

        // [SwaggerOperation(Summary = "Edit a building")]
        // [HttpPut]
        // public async Task<ActionResult> Put(int id, [FromBody] Building building)
        // {
        //     if (id != building.BuildingId)
        //     {
        //         return BadRequest();
        //     }

        //     await _buildingRepository.Update(building);

        //     return NoContent();
        // }

        [SwaggerOperation(Summary = "Delete a building")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var buildingToDelete = await _buildingRepository.Get(id);
            if (buildingToDelete == null)
            {
                return NotFound();
            }

            await _buildingRepository.Delete(buildingToDelete.BuildingId);
            return NoContent();
        }

        [SwaggerOperation(Summary = "Get elevators for a building")]
        [HttpDelete("{id}/elevators")]
        public async Task<IEnumerable<ElevatorDTO>> GetElevators(int id)
        {
            var elevators = await _buildingRepository.GetElevators(id);
            return elevators.Select(e => new ElevatorDTO(e, false));
        }
    }
}
