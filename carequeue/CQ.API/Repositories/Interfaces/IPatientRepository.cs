using carequeue.CQ.API.Models.Entities;

namespace carequeue.CQ.API.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();

        Task<Patient?> GetByIdAsync(Guid patientId);

        Task AddAsync(Patient patient);

        Task UpdateAsync(Patient patient);

        Task DeleteAsync(Patient patient);

        Task SaveChangesAsync();

        Task<bool> ExistsAsync(Guid patientId);

        Task<bool> EmailExistsAsync(string email);

        Task<Patient?> GetByEmailAsync(string email);

        Task<IEnumerable<Patient>> GetPatientsByHospitalIdAsync(int hospitalId);

        Task<IEnumerable<Patient>> GetPatientsByCustomerIdAsync(int customerId);
    }
}
