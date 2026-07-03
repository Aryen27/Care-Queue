using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.DTOs.DoctorDTO;
using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Repositories.Interfaces;

namespace carequeue.CQ.API.Services.Validators
{
    public class DoctorAvailabilityValidator
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;

        public DoctorAvailabilityValidator(
            IDoctorRepository doctorRepository,
            IDoctorScheduleRepository doctorScheduleRepository)
        {
            _doctorRepository = doctorRepository;
            _doctorScheduleRepository = doctorScheduleRepository;
        }

        public async Task<ServiceResult> ValidateAvailabilityRequestAsync(
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

        public async Task<ServiceResult> ValidateDoctorAvailabilityAsync(
            Guid doctorId,
            DateTime appointmentDate,
            TimeSpan appointmentTime,
            int durationMinutes)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);

            if (doctor == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "Doctor not found.");
            }

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

            var schedule =
                await _doctorScheduleRepository.GetScheduleByDoctorAndDayAsync(
                    doctorId,
                    appointmentDate.DayOfWeek);

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

            if (appointmentTime < start ||
                appointmentTime.Add(TimeSpan.FromMinutes(durationMinutes)) > end)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Appointment is outside the doctor's working hours.");
            }

            var minutes =
                (appointmentTime - start).TotalMinutes;

            if (minutes % schedule.SlotDurationMinutes != 0)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Appointment time is not aligned with the doctor's schedule.");
            }

            return ServiceResult.Ok();
        }
    }
}