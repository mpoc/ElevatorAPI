using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using ElevatorAPI.Repositories;
using ElevatorAPI.Models;
using ElevatorAPI.Services;

namespace ElevatorAPI.Controllers
{
    [ApiController]
    [Route("api/elevators")]
    public class ElevatorsController : ControllerBase
    {
        private readonly IElevatorManagerService _elevatorManagerService;

        public ElevatorsController(IElevatorManagerService elevatorManagerService) {
            _elevatorManagerService = elevatorManagerService;
        }

        [SwaggerOperation(Summary = "Get an elevator by id")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Elevator>> Get(int id)
        {
            var elevator = await _elevatorManagerService.Get(id);

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
            var elevatorToDelete = await _elevatorManagerService.Get(id);
            if (elevatorToDelete == null)
            {
                return NotFound();
            }

            await _elevatorManagerService.Delete(elevatorToDelete.ElevatorId);
            return NoContent();
        }

        [SwaggerOperation(Summary = "Call an elevator from a floor to a floor")]
        [HttpGet("{id}/call")]
        public async Task<ActionResult> CallElevator(int id, [FromQuery] int from, [FromQuery] int to)
        {
            var elevator = await _elevatorManagerService.GetWithBuilding(id);

            // No elevator with such id
            if (elevator == null)
                return NotFound();

            var floorsWithinRange =
                from >= 0 &&
                from <= elevator.Building.Height &&
                to >= 0 &&
                to <= elevator.Building.Height;

            // The 'from' or 'to' floors are invalid
            if (!floorsWithinRange)
                return BadRequest();

            // Create an ordered list of actions that an elevator should perform when going to a floor
            var elevatorActions = new List<ElevatorActionStage>() {
                ElevatorActionStage.Begin,
                ElevatorActionStage.EnsuringDoorClosed,
                ElevatorActionStage.ReachingOriginFloor,
                ElevatorActionStage.OpeningBoardingDoor,
                ElevatorActionStage.WaitingToBoard,
                ElevatorActionStage.ClosingBoardingDoor,
                ElevatorActionStage.ReachingDestinationFloor,
                ElevatorActionStage.OpeningAlightingDoor,
                ElevatorActionStage.WaitingToAlight,
                ElevatorActionStage.ClosingAlightingDoor,
                ElevatorActionStage.End
            };
            var elevatorActionsEnumerator = elevatorActions.GetEnumerator();
            // Position 'Current' at first element
            elevatorActionsEnumerator.MoveNext();

            // Elevator is already serving another request.
            // 
            // Not sure how ASP.NET handles concurrent requests. If they can
            // interweave, this could cause race conditions. It would be better
            // to implement a lock.
            if (elevator.Busy)
                return Conflict("The requested elevator is currently busy");

            // Set busy and save
            elevator.Busy = true;
            await _elevatorManagerService.Update(elevator);

            // Go from the specified floor to the specified floor. The exact actions that will happen:
            // 1. Ensure the elevator door is closed
            // 2. Go to the origin floor
            // 3. Open door
            // 4. Wait for 1 second to allow people to board the elevator
            // 5. Close door
            // 6. Go to the destination floor
            // 7. Open door
            // 8. Wait for 1 second to allow people to alight the elevator
            // 9. Close door
            await _elevatorManagerService.CallToFloor(elevator, from, to, elevatorActionsEnumerator);

            // Set not busy and save
            elevator.Busy = false;
            await _elevatorManagerService.Update(elevator);

            return Ok();
        }
    }
}
