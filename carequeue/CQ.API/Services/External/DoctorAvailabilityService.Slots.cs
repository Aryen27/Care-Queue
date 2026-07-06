using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.Entities;

namespace carequeue.CQ.API.Services.External
{
    public partial class DoctorAvailabilityService
    {
        public IEnumerable<TimeSpan> GetAvailableSlots(
            DoctorSchedule schedule,
            IEnumerable<Appointment> appointments,
            int durationMinutes)
        {

            var slots = GenerateSlots(schedule, durationMinutes);

            return slots.Where(slot =>
                !HasAppointmentConflict(
                slot,
                durationMinutes,
                appointments));
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

        // Read-only to store DB Query results in Memory
        private sealed class DoctorAvailabilityContext
        {
            public required IEnumerable<Doctor> Doctors { get; init; }

            public required Dictionary<Guid, DoctorSchedule> Schedules { get; init; }

            public required Dictionary<Guid, List<Appointment>> Appointments { get; init; }
        }
    }
}