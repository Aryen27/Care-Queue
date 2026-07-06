using carequeue.CQ.API.Data;
using carequeue.CQ.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Repositories
{
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        private readonly AppDbContext _context;

        public DoctorScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorSchedule>> GetAllAsync()
        {
            return await _context.DoctorSchedules
                .Include(ds => ds.Doctor)
                .ToListAsync();
        }

        public async Task<DoctorSchedule?> GetByIdAsync(Guid doctorScheduleId)
        {
            return await _context.DoctorSchedules
                .Include(ds => ds.Doctor)
                .FirstOrDefaultAsync(ds => ds.DoctorScheduleId == doctorScheduleId);
        }

        public async Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorIdAsync(Guid doctorId)
        {
            return await _context.DoctorSchedules
                .Where(ds => ds.DoctorId == doctorId)
                .OrderBy(ds => ds.DayOfWeek)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorsAndDayBulkAsync(
            IEnumerable<Guid> doctorIds,
            DayOfWeek dayOfWeek)
        {
            return await _context.DoctorSchedules
                .Where(ds =>
                    doctorIds.Contains(ds.DoctorId) &&
                    ds.DayOfWeek == dayOfWeek)
                .ToListAsync();
        }

        public async Task<DoctorSchedule?> GetScheduleByDoctorAndDayAsync(
            Guid doctorId,
            DayOfWeek dayOfWeek)
        {
            return await _context.DoctorSchedules
                .FirstOrDefaultAsync(ds =>
                    ds.DoctorId == doctorId &&
                    ds.DayOfWeek == dayOfWeek);
        }

        public async Task<bool> ExistsAsync(Guid doctorScheduleId)
        {
            return await _context.DoctorSchedules
                .AnyAsync(ds => ds.DoctorScheduleId == doctorScheduleId);
        }

        public async Task AddAsync(DoctorSchedule doctorSchedule)
        {
            await _context.DoctorSchedules.AddAsync(doctorSchedule);
        }

        public Task UpdateAsync(DoctorSchedule doctorSchedule)
        {
            _context.DoctorSchedules.Update(doctorSchedule);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(DoctorSchedule doctorSchedule)
        {
            _context.DoctorSchedules.Remove(doctorSchedule);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
