using ElevatorAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorAPI.Repositories
{
    public interface ILogRepository
    {
        Task<IEnumerable<Log>> Get();
        Task<IEnumerable<Log>> GetWithElevators();
        Task<Log> Get(int id);
        Task<IEnumerable<Log>> GetByElevator(int elevatorId);
        Task<Log> CreateElevatorCalledLogEntry(Elevator elevator, int fromFloor, int toFloor);
        Task<Log> CreateElevatorDoorStatusChangeLogEntry(Elevator elevator, DoorStatus fromStatus, DoorStatus toStatus);
        Task<Log> CreateElevatorMovedEntry(Elevator elevator, int fromFloor, int toFloor);
        Task Update(Log log);
        Task Delete(int id);
    }
}
