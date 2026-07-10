using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Models.Enums;

namespace carequeue.CQ.API.Repositories.Interfaces
{
    public interface INotificationTemplateRepository
    {
        Task<IEnumerable<NotificationTemplate>> GetAllAsync();

        Task<NotificationTemplate?> GetByTypeAsync(string type);

        Task AddAsync(NotificationTemplate template);

        Task UpdateAsync(NotificationTemplate template);

        Task DeleteAsync(NotificationTemplate template);

        Task SaveChangesAsync();

        // Validation Queries
        Task<bool> ExistsAsync(int templateId);

        Task<bool> NameExistsAsync(string name);

        // Search Queries
        Task<NotificationTemplate?> GetByNameAsync(string name);

        Task<IEnumerable<NotificationTemplate>> GetByTypeAsync(TemplateType type);

        Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync();
    }
}
