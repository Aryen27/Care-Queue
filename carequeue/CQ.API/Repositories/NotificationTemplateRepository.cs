using carequeue.CQ.API.Data;
using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Models.Enums;
using carequeue.CQ.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Repositories
{
    public class NotificationTemplateRepository : INotificationTemplateRepository
    {
        private readonly AppDbContext _context;

        public NotificationTemplateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NotificationTemplate>> GetAllAsync()
        {
            return await _context.NotificationTemplates
                .ToListAsync();
        }
        public async Task<IEnumerable<NotificationTemplate>> GetByTypeAsync(TemplateType type)
        {
            return await _context.NotificationTemplates
                .AsNoTracking()
                .Where(t => t.Type == type)
                .ToListAsync();
        }

        public async Task<NotificationTemplate?> GetByNameAsync(string name)
        {
            return await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<NotificationTemplate?> GetByTypeAsync(string type)
        {
            if (Enum.TryParse<TemplateType>(type, true, out var parsedType))
            {
                return await _context.NotificationTemplates
                    .FirstOrDefaultAsync(t => t.Type == parsedType);
            }

            return null;
        }

        public async Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync()
        {
            return await _context.NotificationTemplates
                .Where(t => t.IsActive)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int templateId)
        {
            return await _context.NotificationTemplates
                .AnyAsync(t => t.TemplateId == templateId);
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _context.NotificationTemplates
                .AnyAsync(t => t.Name == name);
        }

        public async Task<NotificationTemplate?> GetByIdAsync(int templateId)
        {
            return await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.TemplateId == templateId);
        }

        public async Task AddAsync(NotificationTemplate template)
        {
            await _context.NotificationTemplates.AddAsync(template);
        }

        public Task UpdateAsync(NotificationTemplate template)
        {
            _context.NotificationTemplates.Update(template);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(NotificationTemplate template)
        {
            _context.NotificationTemplates.Remove(template);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
