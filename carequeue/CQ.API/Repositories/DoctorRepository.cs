using carequeue.CQ.API.Data;
using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext _context;

        public DoctorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.Hospital)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(Guid doctorId)
        {
            return await _context.Doctors
                .Include(d => d.Hospital)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        }

        public async Task<Doctor?> GetByEmailAsync(string email)
        {
            return await _context.Doctors
                .Include(d => d.Hospital)
                .FirstOrDefaultAsync(d => d.Email == email);
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsByHospitalIdAsync(int hospitalId)
        {
            return await _context.Doctors
                .Where(d => d.HospitalId == hospitalId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid doctorId)
        {
            return await _context.Doctors
                .AnyAsync(d => d.DoctorId == doctorId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Doctors
                .AnyAsync(d => d.Email == email);
        }

        public async Task AddAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
        }

        public Task UpdateAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Doctor doctor)
        {
            _context.Doctors.Remove(doctor);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsByHospitalAndSpecializationAsync(
    int hospitalId,
    string specialization)
        {
            return await _context.Doctors.Where(d =>
                    d.HospitalId == hospitalId &&
                    d.Specialization.Trim().ToLower() == specialization.Trim().ToLower() &&
                    d.IsActive &&
                    d.IsAvailable)
                    .ToListAsync();
        }
    }
}