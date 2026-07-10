using carequeue.CQ.API.Data;
using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.Hospital)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Customer)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Hospital)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByHospitalIdAsync(int hospitalId)
        {
            return await _context.Appointments
                .Where(a => a.HospitalId == hospitalId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByCustomerIdAsync(int customerId)
        {
            return await _context.Appointments
                .Where(a => a.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime appointmentDate)
        {
            return await _context.Appointments
                .Where(a => a.AppointmentDate.Date == appointmentDate.Date)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int appointmentId)
        {
            return await _context.Appointments
                .AnyAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsByDateBulkAsync(
            IEnumerable<Guid> doctorIds,
            DateTime appointmentDate)
        {
            return await _context.Appointments
                .Where(a =>
                    doctorIds.Contains(a.DoctorId) &&
                    a.AppointmentDate.Date == appointmentDate.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsByDateAsync(
            Guid doctorId,
            DateTime date,
            int? excludeAppointmentId)
        {
            return await _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.AppointmentDate.Date == date.Date &&
                    (!excludeAppointmentId.HasValue ||
                    a.AppointmentId != excludeAppointmentId)).ToListAsync();
        }

        public async Task<bool> AppointmentExistsAsync(
            Guid doctorId,
            DateTime appointmentDate,
            TimeSpan appointmentTime)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == appointmentDate.Date &&
                a.AppointmentTime == appointmentTime);
        }

        public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsByDateAsync(
            Guid doctorId,
            DateTime appointmentDate)
        {
            return await _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.AppointmentDate.Date == appointmentDate.Date)
                .ToListAsync();
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
