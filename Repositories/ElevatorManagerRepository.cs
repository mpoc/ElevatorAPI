using System.Reflection.Metadata;
using ElevatorAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace ElevatorAPI.Repositories
{
    public class ElevatorManagerRepository : IElevatorManagerRepository
    {
        private readonly ElevatorAPIContext _context;

        public ElevatorManagerRepository(ElevatorAPIContext context)
        {
            _context = context;
        }

        // Creates new log entry for an elevator with a specified message
        private async Task<Log> CreateLogEntry(Elevator elevator, string message)
        {
            Log newLog = new Log
            {
                Date = DateTime.Now,
                Elevator = elevator,
                Info = message
            };

            _context.Logs.Add(newLog);
            await _context.SaveChangesAsync();

            return newLog;
        }

        // Creates a log entry for when an elevator gets called to go from a floor to another floor
        private async Task<Log> CreateElevatorCalledLogEntry(Elevator elevator, int fromFloor, int toFloor)
        {
            return await CreateLogEntry(elevator, $"Elevator has been called from floor {fromFloor} to {toFloor}.");
        }

        // Creates a log entry whenever elevator door status is changed
        private async Task<Log> CreateElevatorDoorStatusChangeLogEntry(Elevator elevator, DoorStatus fromStatus, DoorStatus toStatus)
        {
            return await CreateLogEntry(elevator, $"Elevator door status changed from {fromStatus} to {toStatus}.");
        }

        // Creates a log entry whenever an elevator moves to a floor
        public async Task<Log> CreateElevatorMovedEntry(Elevator elevator, int fromFloor, int toFloor)
        {
            return await CreateLogEntry(elevator, $"Elevator moved from floor {fromFloor} to {toFloor}.");
        }

        // Set door status and save
        private async Task SetDoorStatus(Elevator elevator, DoorStatus status)
        {
            // Store status before modification
            var oldStatus = elevator.DoorStatus;

            // Change
            elevator.DoorStatus = status;
            _context.Entry(elevator).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Log
            await CreateElevatorDoorStatusChangeLogEntry(elevator, oldStatus, status);
        }

        // Open or close door
        private async Task UseDoor(Elevator elevator, DoorStatus status)
        {
            if (elevator.DoorStatus != DoorStatus.Closed && elevator.DoorStatus != DoorStatus.Open)
            {
                // If the door is currently opening or closing
                var message = $"Cannot set elevator id {elevator.ElevatorId} door to {status}, because door is currently {elevator.DoorStatus}";
                throw new Exception(message);
            }
            else if (elevator.DoorStatus != status)
            {
                // If the door is of the opposite status (e.g. open when we want closed)

                // Set and save
                var transitionalStatus = status == DoorStatus.Open ? DoorStatus.Opening : DoorStatus.Closing;
                await SetDoorStatus(elevator, transitionalStatus);

                await Task.Delay(2000);

                // Set and save
                await SetDoorStatus(elevator, status);
            }
        }

        // Moves the elevator a single floor up or down
        private async Task Move(Elevator elevator, ElevatorMoveCommand direction)
        {
            var status = direction == ElevatorMoveCommand.Up ? ElevatorStatus.MovingUp : ElevatorStatus.MovingDown;
            var floorModifier = direction == ElevatorMoveCommand.Up ? 1 : -1;

            // Set and save
            elevator.ElevatorStatus = status;
            _context.Entry(elevator).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Wait 1 second
            await Task.Delay(1000);

            // Store old floor
            var oldFloor = elevator.AtFloor;

            // Set and save the updated floor
            elevator.AtFloor += floorModifier;
            _context.Entry(elevator).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Log
            await CreateElevatorMovedEntry(elevator, oldFloor, elevator.AtFloor);
        }

        public async Task CallToFloor(Elevator elevator, int from, int to, IEnumerator<ElevatorActionStage> stage)
        {
            switch (stage.Current)
            {
                case ElevatorActionStage.Begin:
                    await CreateElevatorCalledLogEntry(elevator, from, to);

                    stage.MoveNext();
                    await CallToFloor(elevator, from, to, stage);
                    break;
                case ElevatorActionStage.EnsuringDoorClosed:
                    await UseDoor(elevator, DoorStatus.Closed);

                    stage.MoveNext();
                    await CallToFloor(elevator, from, to, stage);
                    break;
                case ElevatorActionStage.ReachingOriginFloor:
                    if (elevator.AtFloor == from)
                    {
                        // We have reached the 'from' floor, move on to the next stage

                        // Set and save
                        elevator.ElevatorStatus = ElevatorStatus.Idle;
                        _context.Entry(elevator).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        stage.MoveNext();
                        await CallToFloor(elevator, from, to, stage);
                    }
                    else if (elevator.AtFloor < from)
                    {
                        // We are lower than the 'from' floor

                        await Move(elevator, ElevatorMoveCommand.Up);

                        // Add log entry here

                        await CallToFloor(elevator, from, to, stage);
                    }
                    else if (elevator.AtFloor > from)
                    {
                        // We are higher than the 'from' floor

                        await Move(elevator, ElevatorMoveCommand.Down);

                        // Add log entry here

                        await CallToFloor(elevator, from, to, stage);
                    }
                    break;
                case ElevatorActionStage.OpeningBoardingDoor:
                    await UseDoor(elevator, DoorStatus.Open);

                    stage.MoveNext();
                    await CallToFloor(elevator, from, to, stage);
                    break;
                case ElevatorActionStage.WaitingToBoard:
                    await Task.Delay(1000);

                    stage.MoveNext();
                    await CallToFloor(elevator, from, to, stage);
                    break;
                case ElevatorActionStage.ClosingBoardingDoor:
                    await UseDoor(elevator, DoorStatus.Closed);

                    stage.MoveNext();
                    await CallToFloor(elevator, from, to, stage);
                    break;
                case ElevatorActionStage.ReachingDestinationFloor:
                    if (elevator.AtFloor == to)
                    {
                        // We have reached the 'to' floor, move on to the next stage

                        // Set and save
                        elevator.ElevatorStatus = ElevatorStatus.Idle;
                        _context.Entry(elevator).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        stage.MoveNext();
                        await CallToFloor(elevator, from, to, stage);
                    }
                    else if (elevator.AtFloor < to)
                    {
                        // We are lower than the 'to' floor

                        await Move(elevator, ElevatorMoveCommand.Up);

                        // Add log entry here

                        await CallToFloor(elevator, from, to, stage);
                    }
                    else if (elevator.AtFloor > to)
                    {
                        // We are higher than the 'to' floor

                        await Move(elevator, ElevatorMoveCommand.Down);

                        // Add log entry here

                        await CallToFloor(elevator, from, to, stage);
                    }
                    break;
                case ElevatorActionStage.OpeningAlightingDoor:
                    await UseDoor(elevator, DoorStatus.Open);

                    stage.MoveNext();
                    await CallToFloor(elevator, from, to, stage);
                    break;
                case ElevatorActionStage.WaitingToAlight:
                    await Task.Delay(1000);

                    stage.MoveNext();
                    await CallToFloor(elevator, from, to, stage);
                    break;
                case ElevatorActionStage.ClosingAlightingDoor:
                    await UseDoor(elevator, DoorStatus.Closed);

                    stage.MoveNext();
                    await CallToFloor(elevator, from, to, stage);
                    break;
                case ElevatorActionStage.End:
                    return;
                default:
                    throw new Exception($"Invalid elevator action stage");
            }
        }
    }
}
