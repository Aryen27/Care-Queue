using carequeue.CQ.API.Models.Entites;

namespace carequeue.CQ.API.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(int customerId);

        Task AddAsync(Customer customer);

        Task UpdateAsync(Customer customer);

        Task DeleteAsync(Customer customer);

        Task SaveChangesAsync();

        Task<bool> ExistsAsync(int customerId);

        Task<bool> EmailExistsAsync(string email);

        Task<Customer?> GetByEmailAsync(string email);

        Task<IEnumerable<Patient>> GetPatientsByCustomerIdAsync(int customerId);
    }
}
