using ElevatorAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorAPI.Repositories
{
    public interface IElevatorManagerRepository
    {
        Task CallToFloor(Elevator elevator, int from, int to, IEnumerator<ElevatorActionStage> stage);
    }
}
