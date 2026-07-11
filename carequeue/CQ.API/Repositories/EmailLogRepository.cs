using carequeue.CQ.API.Data;
using carequeue.CQ.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Repositories
{
    public class EmailLogRepository
    {
        private readonly AppDbContext _context;

        public EmailLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EmailLog?> GetByIdAsync(int emailLogId)
        {
            return await _context.EmailLogs
                .Include(e => e.Notification)
                .FirstOrDefaultAsync(e => e.EmailLogId == emailLogId);
        }

        public async Task<EmailLog?> GetByNotificationIdAsync(int notificationId)
        {
            return await _context.EmailLogs
                .Include(e => e.Notification)
                .FirstOrDefaultAsync(e => e.NotificationId == notificationId);
        }

        public async Task<IEnumerable<EmailLog>> GetAllAsync()
        {
            return await _context.EmailLogs
                .Include(e => e.Notification)
                .OrderByDescending(e => e.SentAt)
                .ToListAsync();
        }

        public async Task AddAsync(EmailLog emailLog)
        {
            await _context.EmailLogs.AddAsync(emailLog);
        }

        public Task UpdateAsync(EmailLog emailLog)
        {
            _context.EmailLogs.Update(emailLog);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(EmailLog emailLog)
        {
            _context.EmailLogs.Remove(emailLog);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}