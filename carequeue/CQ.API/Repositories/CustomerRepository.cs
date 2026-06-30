using carequeue.CQ.API.Data;
using carequeue.CQ.API.Models.Entites;
using carequeue.CQ.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Include(c => c.Patients)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.Patients)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers
                .Include(c => c.Patients)
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<IEnumerable<Patient>> GetPatientsByCustomerIdAsync(int customerId)
        {
            return await _context.Patients
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int customerId)
        {
            return await _context.Customers
                .AnyAsync(c => c.CustomerId == customerId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Customers
                .AnyAsync(c => c.Email == email);
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
