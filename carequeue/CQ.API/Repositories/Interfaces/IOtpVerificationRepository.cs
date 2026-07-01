using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Models.Enums;

namespace carequeue.CQ.API.Repositories.Interfaces
{
    public interface IOtpVerificationRepository
    {
        // CRUD
        Task<IEnumerable<OtpVerification>> GetAllAsync();

        Task<OtpVerification?> GetByIdAsync(int otpId);

        Task AddAsync(OtpVerification otpVerification);

        Task UpdateAsync(OtpVerification otpVerification);

        Task DeleteAsync(OtpVerification otpVerification);

        Task SaveChangesAsync();

        // Validation Queries
        Task<bool> ExistsAsync(int otpId);

        // Search Queries
        Task<IEnumerable<OtpVerification>> GetByCustomerIdAsync(int customerId);

        Task<IEnumerable<OtpVerification>> GetByPurposeAsync(OtpPurpose purpose);

        Task<OtpVerification?> GetLatestOtpAsync(
            int customerId,
            OtpPurpose purpose);

        Task<IEnumerable<OtpVerification>> GetExpiredOtpsAsync();

        Task<IEnumerable<OtpVerification>> GetUnusedOtpsAsync();
    }
}
