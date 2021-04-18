using ElevatorAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorAPI.Repositories
{
    public class ElevatorRepository : IElevatorRepository
    {
        private readonly ElevatorAPIContext _context;

        public ElevatorRepository(ElevatorAPIContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Log>> Get()
        {
            return await _context.Logs.ToListAsync();
        }

        public async Task<Log> Get(int id)
        {
            return await _context.Logs.FindAsync(id);
        }

        public async Task<IEnumerable<Log>> GetByElevator(int elevatorId)
        {
            return await _context.Logs.Where(log => log.Elevator.Id == elevatorId);
        }

        // Creates a log entry for when an elevator gets called to go from a floor to another floor
        Task<Log> CreateElevatorCalledEntry(int elevatorId, int fromFloor, int toFloor)
        {
            var elevator = await _context.Elevators.FindAsync(elevatorId);
            Log newLog = new Log
            {
                Date = DateTime.Now,
                Elevator = elevator,
                Info = $"Elevator id {elevator.Id} has been called from floor {fromFloor} to {toFloor}."
            };

            _context.Logs.Add(newLog);
            await _context.SaveChangesAsync();

            return newLog;
        }

        // Creates a log entry for when an elevator doors change status
        Task<Log> CreateElevatorDoorStatusChangedEntry(int elevatorId, DoorStatus fromDoorStatus, DoorStatus toDoorStatus)
        {
            var elevator = await _context.Elevators.FindAsync(elevatorId);
            Log newLog = new Log
            {
                Date = DateTime.Now,
                Elevator = elevator,
                Info = $"Elevator id {elevator.Id} doors changed from {fromDoorStatus} to {toDoorStatus}."
            };

            _context.Logs.Add(newLog);
            await _context.SaveChangesAsync();

            return newLog;
        }

        // Creates a log entry for when an elevator arrives at a certain floor (toFloor)
        Task<Log> CreateElevatorMovedEntry(int elevatorId, int fromFloor, int toFloor)
        {
            var elevator = await _context.Elevators.FindAsync(elevatorId);
            Log newLog = new Log
            {
                Date = DateTime.Now,
                Elevator = elevator,
                Info = $"Elevator id {elevator.Id} moved from floor {fromFloor} to {toFloor}."
            };

            _context.Logs.Add(newLog);
            await _context.SaveChangesAsync();

            return newLog;
        }

        // public async Task<Log> Create(Log log)
        // {
        //     _context.Logs.Add(log);
        //     await _context.SaveChangesAsync();

        //     return log;
        // }

        public async Task Delete(int id)
        {
            var logToDelete = await _context.Logs.FindAsync(id);
            _context.Logs.Remove(logToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Log log)
        {
            _context.Entry(log).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
