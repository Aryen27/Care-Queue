using carequeue.CQ.API.Models.Entites;

namespace carequeue.CQ.API.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        // CRUD
        Task<IEnumerable<Appointment>> GetAllAsync();

        Task<Appointment?> GetByIdAsync(int appointmentId);

        Task AddAsync(Appointment appointment);

        Task UpdateAsync(Appointment appointment);

        Task DeleteAsync(Appointment appointment);

        Task SaveChangesAsync();

        // Validation Queries
        Task<bool> ExistsAsync(int appointmentId);

        // Search Queries
        Task<IEnumerable<Appointment>> GetAppointmentsByHospitalIdAsync(int hospitalId);

        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId);

        Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId);

        Task<IEnumerable<Appointment>> GetAppointmentsByCustomerIdAsync(int customerId);

        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime appointmentDate);

        // Scheduling Queries
        Task<bool> AppointmentExistsAsync(
            Guid doctorId,
            DateTime appointmentDate,
            TimeSpan appointmentTime);

        Task<IEnumerable<Appointment>> GetDoctorAppointmentsByDateAsync(
            Guid doctorId,
            DateTime appointmentDate);
    }
}
