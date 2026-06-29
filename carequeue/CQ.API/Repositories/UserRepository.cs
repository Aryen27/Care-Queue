using carequeue.CQ.API.Data;
using carequeue.CQ.API.Models.Entites;
using carequeue.CQ.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Hospital)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByHospitalIdAsync(int hospitalId)
        {
            return await _context.Users
                .Where(u => u.HospitalId == hospitalId)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Hospital)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Hospital)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users
                .AnyAsync(u => u.UserId == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }
        public async Task<bool> IsActiveAsync(int userId)
        {
            return await _context.Users
                .AnyAsync(u => u.UserId == userId && u.IsActive);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}