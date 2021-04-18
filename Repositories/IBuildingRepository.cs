using ElevatorAPI.Models;
using ElevatorAPI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorAPI.Repositories
{
    public interface IBuildingRepository
    {
        Task<IEnumerable<Building>> Get();
        Task<Building> Get(int id);
        Task<IEnumerable<Elevator>> GetElevators(int id);
        Task<Building> Create(Building building);
        Task<Building> CreateWithElevators(CreateBuildingWithElevatorsResource resource);
        Task Update(Building building);
        Task Delete(int id);
    }
}
