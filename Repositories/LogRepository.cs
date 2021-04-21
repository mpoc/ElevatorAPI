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
        public async Task<Log> CreateElevatorCalledLogEntry(Elevator elevator, int fromFloor, int toFloor)
        {
            return await CreateLogEntry(elevator, $"Elevator has been called from floor {fromFloor} to {toFloor}.");
        }

        // Creates a log entry whenever elevator door status is changed
        public async Task<Log> CreateElevatorDoorStatusChangeLogEntry(Elevator elevator, DoorStatus fromStatus, DoorStatus toStatus)
        {
            return await CreateLogEntry(elevator, $"Elevator door status changed from {fromStatus} to {toStatus}.");
        }

        // Creates a log entry whenever an elevator moves to a floor
        public async Task<Log> CreateElevatorMovedEntry(Elevator elevator, int fromFloor, int toFloor)
        {
            return await CreateLogEntry(elevator, $"Elevator moved from floor {fromFloor} to {toFloor}.");
        }


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
