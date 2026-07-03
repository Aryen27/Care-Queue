using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.Entities;

namespace carequeue.CQ.API.Services.External
{
    public partial class DoctorAvailabilityService
    {
        public async Task<ServiceResult<IEnumerable<TimeSpan>>> GetAvailableSlotsAsync(
            Guid doctorId,
            DateTime appointmentDate,
            int durationMinutes)
        {
            var schedule =
                await _doctorScheduleRepository.GetScheduleByDoctorAndDayAsync(
                    doctorId,
                    appointmentDate.DayOfWeek);

            if (schedule == null)
            {
                return ServiceResult<IEnumerable<TimeSpan>>.Fail(
                    ErrorCodes.NotFound,
                    "Doctor does not have a schedule for the selected day.");
            }

            if (!schedule.IsActive || !schedule.isAvailable)
            {
                return ServiceResult<IEnumerable<TimeSpan>>.Fail(
                    ErrorCodes.Validation,
                    "Doctor is unavailable on the selected day.");
            }

            var appointments =
                await _appointmentRepository.GetDoctorAppointmentsByDateAsync(
                    doctorId,
                    appointmentDate);

            var slots = GenerateSlots(schedule, durationMinutes);

            var availableSlots = slots
                .Where(slot => !HasAppointmentConflict(
                    slot,
                    durationMinutes,
                    appointments))
                .ToList();

            return ServiceResult<IEnumerable<TimeSpan>>
                .Ok(availableSlots);
        }

        private static IEnumerable<TimeSpan> GenerateSlots(
            DoctorSchedule schedule,
            int durationMinutes)
        {
            var slots = new List<TimeSpan>();

            var current = schedule.StartTime.ToTimeSpan();
            var end = schedule.EndTime.ToTimeSpan();

            while (current.Add(TimeSpan.FromMinutes(durationMinutes)) <= end)
            {
                slots.Add(current);

                current = current.Add(
                    TimeSpan.FromMinutes(schedule.SlotDurationMinutes));
            }

            return slots;
        }

        private static bool HasAppointmentConflict(
            TimeSpan requestedStart,
            int requestedDuration,
            IEnumerable<Appointment> appointments)
        {
            var requestedEnd =
                requestedStart.Add(
                    TimeSpan.FromMinutes(requestedDuration));

            foreach (var appointment in appointments)
            {
                var existingStart = appointment.AppointmentTime;

                var existingEnd =
                    existingStart.Add(
                        TimeSpan.FromMinutes(
                            appointment.DurationMinutes));

                var overlap =
                    requestedStart < existingEnd &&
                    requestedEnd > existingStart;

                if (overlap)
                {
                    return true;
                }
            }

            return false;
        }
    }
}