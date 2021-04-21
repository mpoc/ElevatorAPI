using ElevatorAPI.Models;
using ElevatorAPI.Resources;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorAPI.Repositories
{
    public class BuildingRepository : IBuildingRepository
    {
        private readonly ElevatorAPIContext _context;

        public BuildingRepository(ElevatorAPIContext context)
        {
            _context = context;
        }

        public async Task<Building> Create(Building building)
        {
            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();

            return building;
        }

        public async Task<Building> CreateWithElevators(CreateBuildingWithElevatorsResource resource)
        {
            var building = new Building {
                Height = resource.Building.Height
            };
            var elevators = Enumerable.Range(0, resource.NumberOfElevators).Select(x => new Elevator
            {
                DoorStatus = DoorStatus.Closed,
                ElevatorStatus = ElevatorStatus.Idle,
                AtFloor = 0,
                Busy = false,
                Building = building
            }).ToList();
            _context.AddRange(elevators);
            await _context.SaveChangesAsync();
            return building;
        }

        public async Task Delete(int id)
        {
            var buildingToDelete = await _context.Buildings.FindAsync(id);
            _context.Buildings.Remove(buildingToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Building>> Get()
        {
            return await _context.Buildings.ToListAsync();
        }

        public async Task<IEnumerable<Building>> GetWithElevators()
        {
            return await _context.Buildings.Include(b => b.Elevators).ToListAsync();
        }

        public async Task<Building> Get(int id)
        {
            return await _context.Buildings.FindAsync(id);
        }

        public async Task<Building> GetWithElevators(int id)
        {
            return await _context.Buildings.Include(b => b.Elevators).FirstOrDefaultAsync(b => b.BuildingId == id);
        }

        public async Task<IEnumerable<Elevator>> GetElevators(int id)
        {
            return await _context.Elevators.Where(e => e.Building.BuildingId == id).ToListAsync();
        }

        public async Task Update(Building building)
        {
            _context.Entry(building).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
