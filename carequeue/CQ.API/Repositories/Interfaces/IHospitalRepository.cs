using carequeue.CQ.API.Models.Entities;

namespace carequeue.CQ.API.Repositories.Interfaces
{
    public interface IHospitalRepository
    {
        Task<IEnumerable<Hospital>> GetAllAsync();
        Task<Hospital?> GetByIdAsync(int id);

        Task AddAsync(Hospital hospital);

        Task UpdateAsync(Hospital hospital);

        Task DeleteAsync(Hospital hospital);

        Task SaveChangesAsync();

        Task<bool> ExistsAsync(int id);
    }
}
