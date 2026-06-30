using carequeue.CQ.API.Data;
using carequeue.CQ.API.Models.Entites;
using carequeue.CQ.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Repositories
{
    public class HospitalRepository : IHospitalRepository
    {
        private readonly AppDbContext _context;

        public HospitalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hospital>> GetAllAsync()
        {
            return await _context.Hospitals.ToListAsync();
        }

        public async Task<Hospital?> GetByIdAsync(int id)
        {
            return await _context.Hospitals
                .FirstOrDefaultAsync(h => h.HospitalId == id);
        }

        public async Task AddAsync(Hospital hospital)
        {
            await _context.Hospitals.AddAsync(hospital);
        }

        public Task UpdateAsync(Hospital hospital)
        {
            _context.Hospitals.Update(hospital);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Hospital hospital)
        {
            _context.Hospitals.Remove(hospital);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Hospitals
                .AnyAsync(h => h.HospitalId == id);
        }
    }
}
