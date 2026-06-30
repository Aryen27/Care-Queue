using carequeue.CQ.API.Data;
using carequeue.CQ.API.Models.Entites;
using carequeue.CQ.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .Include(p => p.Hospital)
                .Include(p => p.Customer)
                .ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(Guid patientId)
        {
            return await _context.Patients
                .Include(p => p.Hospital)
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        public async Task<Patient?> GetByEmailAsync(string email)
        {
            return await _context.Patients
                .Include(p => p.Hospital)
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<IEnumerable<Patient>> GetPatientsByHospitalIdAsync(int hospitalId)
        {
            return await _context.Patients
                .Where(p => p.HospitalId == hospitalId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetPatientsByCustomerIdAsync(int customerId)
        {
            return await _context.Patients
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid patientId)
        {
            return await _context.Patients
                .AnyAsync(p => p.PatientId == patientId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Patients
                .AnyAsync(p => p.Email == email);
        }

        public async Task AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
        }

        public Task UpdateAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Patient patient)
        {
            _context.Patients.Remove(patient);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
