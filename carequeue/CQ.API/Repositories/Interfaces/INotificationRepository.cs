using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Models.Enums;

namespace carequeue.CQ.API.Repositories.Interfaces
{
        public interface INotificationRepository
        {
            Task<IEnumerable<Notification>> GetAllAsync();

            Task<Notification?> GetByIdAsync(int notificationId);

            Task AddAsync(Notification notification);

            Task UpdateAsync(Notification notification);

            Task DeleteAsync(Notification notification);

            Task SaveChangesAsync();

            // Validation Queries
            Task<bool> ExistsAsync(int notificationId);

            // Search Queries
            Task<IEnumerable<Notification>> GetByHospitalIdAsync(int hospitalId);

            Task<IEnumerable<Notification>> GetByCustomerIdAsync(int customerId);

            Task<IEnumerable<Notification>> GetByPatientIdAsync(Guid patientId);

            Task<IEnumerable<Notification>> GetByAppointmentIdAsync(int appointmentId);

            Task<IEnumerable<Notification>> GetByStatusAsync(NotificationStatus status);

            Task<IEnumerable<Notification>> GetByChannelAsync(NotificationChannel channel);

            Task<IEnumerable<Notification>> GetPendingNotificationsAsync();
        }
}
