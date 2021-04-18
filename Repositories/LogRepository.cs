using ElevatorAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorAPI.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly ElevatorAPIContext _context;

        public LogRepository(ElevatorAPIContext context)
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

        // Gets all the logs associated with a certain elevator
        public async Task<IEnumerable<Log>> GetByElevator(int elevatorId)
        {
            return await _context.Logs.Where(log => log.Elevator.ElevatorId == elevatorId).ToListAsync();
        }

        // Creates a log entry for when an elevator gets called to go from a floor to another floor
        public async Task<Log> CreateElevatorCalledEntry(int elevatorId, int fromFloor, int toFloor)
        {
            var elevator = await _context.Elevators.FindAsync(elevatorId);
            Log newLog = new Log
            {
                Date = DateTime.Now,
                Elevator = elevator,
                Info = $"Elevator id {elevator.ElevatorId} has been called from floor {fromFloor} to {toFloor}."
            };

            _context.Logs.Add(newLog);
            await _context.SaveChangesAsync();

            return newLog;
        }

        // Creates a log entry for when an elevator doors change status
        public async Task<Log> CreateElevatorDoorStatusChangedEntry(int elevatorId, DoorStatus fromDoorStatus, DoorStatus toDoorStatus)
        {
            var elevator = await _context.Elevators.FindAsync(elevatorId);
            Log newLog = new Log
            {
                Date = DateTime.Now,
                Elevator = elevator,
                Info = $"Elevator id {elevator.ElevatorId} doors changed from {fromDoorStatus} to {toDoorStatus}."
            };

            _context.Logs.Add(newLog);
            await _context.SaveChangesAsync();

            return newLog;
        }

        // Creates a log entry for when an elevator arrives at a certain floor (toFloor)
        public async Task<Log> CreateElevatorMovedEntry(int elevatorId, int fromFloor, int toFloor)
        {
            var elevator = await _context.Elevators.FindAsync(elevatorId);
            Log newLog = new Log
            {
                Date = DateTime.Now,
                Elevator = elevator,
                Info = $"Elevator id {elevator.ElevatorId} moved from floor {fromFloor} to {toFloor}."
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
