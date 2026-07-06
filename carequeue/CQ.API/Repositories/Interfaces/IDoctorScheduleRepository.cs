namespace carequeue.CQ.API.Repositories.Interfaces
{
    public interface IDoctorScheduleRepository
    {
        Task<IEnumerable<DoctorSchedule>> GetAllAsync();

        Task<DoctorSchedule?> GetByIdAsync(Guid doctorScheduleId);

        Task AddAsync(DoctorSchedule doctorSchedule);

        Task UpdateAsync(DoctorSchedule doctorSchedule);

        Task DeleteAsync(DoctorSchedule doctorSchedule);

        Task SaveChangesAsync();

        // Validation Queries
        Task<bool> ExistsAsync(Guid doctorScheduleId);

        // Search Queries
        Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorIdAsync(Guid doctorId);

        //Bulk Search 
        Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorsAndDayBulkAsync(
        IEnumerable<Guid> doctorIds,
        DayOfWeek dayOfWeek);

        Task<DoctorSchedule?> GetScheduleByDoctorAndDayAsync(
            Guid doctorId,
            DayOfWeek dayOfWeek);
    }
}
