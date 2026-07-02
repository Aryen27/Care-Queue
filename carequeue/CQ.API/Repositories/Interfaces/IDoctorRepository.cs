using carequeue.CQ.API.Models.Entities;

namespace carequeue.CQ.API.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync();

        Task<Doctor?> GetByIdAsync(Guid doctorId);

        Task AddAsync(Doctor doctor);

        Task UpdateAsync(Doctor doctor);

        Task DeleteAsync(Doctor doctor);

        Task SaveChangesAsync();

        Task<bool> ExistsAsync(Guid doctorId);

        Task<bool> EmailExistsAsync(string email);
        Task<Doctor?> GetByEmailAsync(string email);

        Task<IEnumerable<Doctor>> GetDoctorsByHospitalIdAsync(int hospitalId);

        Task<IEnumerable<Doctor>> GetDoctorsByHospitalAndSpecializationAsync(int hospitalId, string specialization);
    }
}