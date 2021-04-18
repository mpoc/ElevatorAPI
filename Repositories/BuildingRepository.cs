using ElevatorAPI.Models;
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

        public async Task<Building> Get(int id)
        {
            return await _context.Buildings.FindAsync(id);
        }

        public async Task Update(Building building)
        {
            _context.Entry(building).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}