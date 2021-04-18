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
        private readonly ElevatorContext _context;

        public ElevatorRepository(ElevatorContext context)
        {
            _context = context;
        }

        public async Task<Elevator> Create(Elevator elevator)
        {
            _context.Elevators.Add(elevator);
            await _context.SaveChangesAsync();

            return elevator;
        }

        public async Task Delete(int id)
        {
            var elevatorToDelete = await _context.Elevators.FindAsync(id);
            _context.Elevators.Remove(elevatorToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Elevator>> Get()
        {
            return await _context.Elevators.ToListAsync();
        }

        public async Task<Elevator> Get(int id)
        {
            return await _context.Elevators.FindAsync(id);
        }

        public async Task Update(Elevator elevator)
        {
            _context.Entry(elevator).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
