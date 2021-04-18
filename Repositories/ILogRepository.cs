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
        Task<Log> Get(int id);
        Task<IEnumerable<Log>> GetByElevator(int elevatorId);
        Task<Log> CreateElevatorCalledEntry(int elevatorId, int fromFloor, int toFloor);
        Task<Log> CreateElevatorDoorStatusChangedEntry(int elevatorId, DoorStatus fromDoorStatus, DoorStatus toDoorStatus);
        Task<Log> CreateElevatorMovedEntry(int elevatorId, int fromFloor, int toFloor);
        Task Update(Log log);
        Task Delete(int id);
    }
}
