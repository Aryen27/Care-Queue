using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.Entities;

namespace carequeue.CQ.API.Services.Validators
{
    public class AppointmentAvailabilityValidator
    {
        public ServiceResult ValidateAvailability(
            Doctor doctor,
            DoctorSchedule? schedule,
            IEnumerable<Appointment> appointments,
            int? ignoredAppointmentId,
            DateTime appointmentDate,
            TimeSpan appointmentTime,
            int durationMinutes)
        {
            if (!doctor.IsActive)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Doctor is inactive.");
            }

            if (!doctor.IsAvailable)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Doctor is unavailable.");
            }

            if (schedule == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Doctor does not work on the selected day.");
            }

            if (!schedule.IsActive || !schedule.isAvailable)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Doctor is unavailable on the selected day.");
            }

            var scheduleStart =
                schedule.StartTime.ToTimeSpan();

            var scheduleEnd =
                schedule.EndTime.ToTimeSpan();

            var requestedEnd =
                appointmentTime.Add(
                    TimeSpan.FromMinutes(durationMinutes));

            if (appointmentTime < scheduleStart ||
                requestedEnd > scheduleEnd)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Appointment is outside the doctor's working hours.");
            }

            var offset =
                (appointmentTime - scheduleStart).TotalMinutes;

            if (offset % schedule.SlotDurationMinutes != 0)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Appointment time is not aligned with the doctor's schedule.");
            }

            foreach (var existing in appointments)
            {
                if (existing.Status == Models.Enums.AppointmentStatus.Cancelled)
                {
                    continue;
                }

                if (ignoredAppointmentId.HasValue && existing.AppointmentId == ignoredAppointmentId.Value)
                {
                    continue;
                }

                var existingStart = existing.AppointmentTime;

                var existingEnd =
                    existingStart.Add(
                        TimeSpan.FromMinutes(existing.DurationMinutes));

                bool overlap =
                    appointmentTime < existingEnd &&
                    requestedEnd > existingStart;

                if (overlap)
                {
                    return ServiceResult.Fail(
                        ErrorCodes.Conflict,
                        "Doctor already has an appointment during the selected time.");
                }
            }

            return ServiceResult.Ok();
        }
    }
}