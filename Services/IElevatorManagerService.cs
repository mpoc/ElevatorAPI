using ElevatorAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorAPI.Services
{
    public interface IElevatorManagerService
    {
        Task CallToFloor(Elevator elevator, int from, int to, IEnumerator<ElevatorActionStage> stage);
        Task<IEnumerable<Elevator>> Get();
        Task<Elevator> Get(int id);
        Task<Elevator> GetWithBuilding(int id);
        Task<Elevator> Create(Elevator elevator);
        Task Update(Elevator elevator);
        Task Delete(int id);
    }
}
