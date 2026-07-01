using carequeue.CQ.API.Models.Entities;

namespace carequeue.CQ.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<IEnumerable<User>> GetUsersByHospitalIdAsync(int hospitalId);
        Task<User?> GetByIdAsync(int id);

        Task<User?> GetByEmailAsync(string email);

        Task<bool> ExistsAsync(int id);

        Task<bool> EmailExistsAsync(string email);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(User user);

        Task<bool> IsActiveAsync(int userId);
        
        Task SaveChangesAsync();
    }
}