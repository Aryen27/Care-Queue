using carequeue.CQ.API.Data;
using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Models.Enums;
using carequeue.CQ.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .Include(n => n.Hospital)
                .Include(n => n.Customer)
                .Include(n => n.Patient)
                .Include(n => n.Appointment)
                .Include(n => n.Template)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(int notificationId)
        {
            return await _context.Notifications
                .Include(n => n.Hospital)
                .Include(n => n.Customer)
                .Include(n => n.Patient)
                .Include(n => n.Appointment)
                .Include(n => n.Template)
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);
        }

        public async Task<IEnumerable<Notification>> GetByHospitalIdAsync(int hospitalId)
        {
            return await _context.Notifications
                .Where(n => n.HospitalId == hospitalId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Notifications
                .Where(n => n.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.Notifications
                .Where(n => n.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _context.Notifications
                .Where(n => n.AppointmentId == appointmentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetByStatusAsync(NotificationStatus status)
        {
            return await _context.Notifications
                .Where(n => n.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetByChannelAsync(NotificationChannel channel)
        {
            return await _context.Notifications
                .Where(n => n.Channel == channel)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetPendingNotificationsAsync()
        {
            return await _context.Notifications
                .Where(n =>
                n.Status == NotificationStatus.Pending &&
                (
                    n.NextRetryAt == null ||
                    n.NextRetryAt <= DateTime.UtcNow
                ))
                .OrderBy(n => n.CreatedAt).ToListAsync();
        }

        public async Task<bool> ExistsAsync(int notificationId)
        {
            return await _context.Notifications
                .AnyAsync(n => n.NotificationId == notificationId);
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Notification notification)
        {
            _context.Notifications.Remove(notification);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
