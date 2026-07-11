using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.DTOs.DoctorDTO;
using carequeue.CQ.API.Models.Entities;

namespace carequeue.CQ.API.Services.Validators
{
    public class DoctorAvailabilityValidator
    {
        public ServiceResult ValidateAvailabilityRequest(
            DoctorAvailabilityRequestDto request)
        {
            var errors = new List<ValidationError>();

            if (request.HospitalId <= 0)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(request.HospitalId),
                    Message = "Hospital is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Specialization))
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(request.Specialization),
                    Message = "Specialization is required."
                });
            }

            if (request.AppointmentDate.Date < DateTime.Today)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(request.AppointmentDate),
                    Message = "Appointment date cannot be in the past."
                });
            }

            if (request.DurationMinutes != 30 &&
                request.DurationMinutes != 60)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(request.DurationMinutes),
                    Message = "Appointment duration must be either 30 or 60 minutes."
                });
            }

            if (errors.Any())
            {
                return ServiceResult.Validation(errors);
            }

            return ServiceResult.Ok();
        }

        public ServiceResult ValidateDoctorAvailability(
            Doctor doctor,
            DoctorSchedule? schedule,
            IEnumerable<Appointment> appointments,
            DoctorAvailabilityRequestDto request)
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

            var start = schedule.StartTime.ToTimeSpan();
            var end = schedule.EndTime.ToTimeSpan();

            if (request.AppointmentTime < start ||
                request.AppointmentTime.Add(
                    TimeSpan.FromMinutes(request.DurationMinutes)) > end)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Appointment is outside the doctor's working hours.");
            }

            var minutes =
                (request.AppointmentTime - start).TotalMinutes;

            if (minutes % schedule.SlotDurationMinutes != 0)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Appointment time is not aligned with the doctor's schedule.");
            }

            foreach (var appointment in appointments)
            {
                var existingStart = appointment.AppointmentTime;
                var existingEnd = existingStart.Add(
                    TimeSpan.FromMinutes(appointment.DurationMinutes));

                var requestedEnd = request.AppointmentTime.Add(
                    TimeSpan.FromMinutes(request.DurationMinutes));

                var overlap =
                    request.AppointmentTime < existingEnd &&
                    requestedEnd > existingStart;

                if (overlap)
                {
                    return ServiceResult.Fail(
                        ErrorCodes.Validation,
                        "Doctor already has an appointment at the selected time.");
                }
            }

            return ServiceResult.Ok();
        }
    }
}