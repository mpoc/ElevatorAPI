using System.Reflection.Metadata;
using ElevatorAPI.Models;
using ElevatorAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace ElevatorAPI.Services
{
    public class ElevatorManagerService : IElevatorManagerService
    {
        private readonly ILogRepository _logRepository;
        private readonly IElevatorRepository _elevatorRepository;

        public ElevatorManagerService(ILogRepository logRepository, IElevatorRepository elevatorRepository)
        {
            _logRepository = logRepository;
            _elevatorRepository = elevatorRepository;
        }

        // Set door status and save
        private async Task SetDoorStatus(Elevator elevator, DoorStatus status)
        {
            // Store status before modification
            var oldStatus = elevator.DoorStatus;

            // Change
            elevator.DoorStatus = status;
            await _elevatorRepository.Update(elevator);

            // Log
            await _logRepository.CreateElevatorDoorStatusChangeLogEntry(elevator, oldStatus, status);
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
            await _elevatorRepository.Update(elevator);

            // Wait 1 second
            await Task.Delay(1000);

            // Store old floor
            var oldFloor = elevator.AtFloor;

            // Set and save the updated floor
            elevator.AtFloor += floorModifier;
            await _elevatorRepository.Update(elevator);

            // Log
            await _logRepository.CreateElevatorMovedEntry(elevator, oldFloor, elevator.AtFloor);
        }

        public async Task CallToFloor(Elevator elevator, int from, int to, IEnumerator<ElevatorActionStage> stage)
        {
            switch (stage.Current)
            {
                case ElevatorActionStage.Begin:
                    await _logRepository.CreateElevatorCalledLogEntry(elevator, from, to);

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
                        await _elevatorRepository.Update(elevator);

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
                        await _elevatorRepository.Update(elevator);

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

        // Below we just proxy the elevator repository methods because the
        // elevator manager service will be the only service injected into
        // elevators controller
        public async Task<IEnumerable<Elevator>> Get() => await _elevatorRepository.Get();
        public async Task<Elevator> Get(int id) => await _elevatorRepository.Get(id);
        public async Task<Elevator> GetWithBuilding(int id) => await _elevatorRepository.GetWithBuilding(id);
        public async Task<Elevator> Create(Elevator elevator) => await _elevatorRepository.Create(elevator);
        public async Task Update(Elevator elevator) => await _elevatorRepository.Update(elevator);
        public async Task Delete(int id) => await _elevatorRepository.Delete(id);
    }
}
