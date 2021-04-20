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
    public class ElevatorRepository : IElevatorRepository
    {
        private readonly ElevatorAPIContext _context;

        public ElevatorRepository(ElevatorAPIContext context)
        {
            _context = context;
        }

        public async Task<Elevator> Create(Elevator elevator)
        {
            _context.Elevators.Add(elevator);
            await _context.SaveChangesAsync();

            return elevator;
        }

        public async Task Delete(int id)
        {
            var elevatorToDelete = await _context.Elevators.FindAsync(id);
            _context.Elevators.Remove(elevatorToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Elevator>> Get()
        {
            return await _context.Elevators.ToListAsync();
        }

        public async Task<Elevator> Get(int id)
        {
            return await _context.Elevators.FindAsync(id);
        }

        public async Task<Elevator> GetWithBuilding(int id)
        {
            return await _context.Elevators.Include(e => e.Building).FirstOrDefaultAsync(e => e.ElevatorId == id);
        }

        // Set door status and save
        private async Task SetDoorStatus(Elevator elevator, DoorStatus status)
        {
            elevator.DoorStatus = status;
            _context.Entry(elevator).State = EntityState.Modified;
            await _context.SaveChangesAsync();
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

            // Set and save the updated floor
            elevator.AtFloor += floorModifier;
            _context.Entry(elevator).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task CallToFloor(Elevator elevator, int from, int to, IEnumerator<ElevatorActionStage> stage)
        {
            switch (stage.Current)
            {
                case ElevatorActionStage.Begin:
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

        public async Task Update(Elevator elevator)
        {
            _context.Entry(elevator).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
