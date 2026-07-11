using carequeue.CQ.API.Data;
using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Models.Enums;
using carequeue.CQ.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Repositories
{
    public class OtpVerificationRepository : IOtpVerificationRepository
    {
        private readonly AppDbContext _context;

        public OtpVerificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OtpVerification>> GetAllAsync()
        {
            return await _context.OtpVerifications
                .Include(o => o.Customer)
                .ToListAsync();
        }

        public async Task<OtpVerification?> GetByIdAsync(int otpId)
        {
            return await _context.OtpVerifications
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OtpId == otpId);
        }

        public async Task<IEnumerable<OtpVerification>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.OtpVerifications
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OtpVerification>> GetByPurposeAsync(OtpPurpose purpose)
        {
            return await _context.OtpVerifications
                .Where(o => o.Purpose == purpose)
                .ToListAsync();
        }

        public async Task<OtpVerification?> GetLatestOtpAsync(
            int customerId,
            OtpPurpose purpose)
        {
            return await _context.OtpVerifications
                .Where(o =>
                    o.CustomerId == customerId &&
                    o.Purpose == purpose)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OtpVerification>> GetExpiredOtpsAsync()
        {
            return await _context.OtpVerifications
                .Where(o => o.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<IEnumerable<OtpVerification>> GetUnusedOtpsAsync()
        {
            return await _context.OtpVerifications
                .Where(o => o.UsedAt == null && o.RevokedAt == null)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int otpId)
        {
            return await _context.OtpVerifications
                .AnyAsync(o => o.OtpId == otpId);
        }

        public async Task AddAsync(OtpVerification otpVerification)
        {
            await _context.OtpVerifications.AddAsync(otpVerification);
        }

        public Task UpdateAsync(OtpVerification otpVerification)
        {
            _context.OtpVerifications.Update(otpVerification);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(OtpVerification otpVerification)
        {
            _context.OtpVerifications.Remove(otpVerification);
            return Task.CompletedTask;
        }

        public async Task<OtpVerification?> GetLatestActiveOtpAsync(
            int customerId,
            OtpPurpose purpose)
        {
            return await _context.OtpVerifications
                .Where(o =>
                    o.CustomerId == customerId &&
                    o.Purpose == purpose &&
                    o.UsedAt == null &&
                    o.RevokedAt == null &&
                    o.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OtpVerification>> GetActiveOtpsAsync(
            int customerId,
            OtpPurpose purpose)
        {
            return await _context.OtpVerifications
                .Where(o =>
                    o.CustomerId == customerId &&
                    o.Purpose == purpose &&
                    o.UsedAt == null &&
                    o.RevokedAt == null &&
                    o.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
