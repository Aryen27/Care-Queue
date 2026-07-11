using carequeue.CQ.API.DTOs.AppointmentDTO;
using carequeue.CQ.API.Mappers;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.Enums;
using carequeue.CQ.API.Services.Events.Events;

namespace carequeue.CQ.API.Services.External
{
    public partial class AppointmentService
    {
        public async Task<ServiceResult<AppointmentReadDto>> CreateAsync(
            AppointmentCreateDto dto,
            int customerId)
        {
            dto.CustomerId = customerId;

            var validation = await _validator.ValidateCreateAsync(dto);

            if (!validation.Success)
            {
                return validation.ToGeneric<AppointmentReadDto>();
            }

            var availability = await ValidateBookingAvailabilityAsync(
                dto.DoctorId,
                dto.AppointmentDate,
                dto.AppointmentTime,
                dto.DurationMinutes);

            if (!availability.Success)
            {
                return availability.ToGeneric<AppointmentReadDto>();
            }

            var appointment = dto.ToEntity(customerId);

            appointment.Status = AppointmentStatus.Scheduled;

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            appointment = await _appointmentRepository.GetByIdAsync(
                appointment.AppointmentId);

            if (appointment == null)
            {
                throw new Exception("Appointment not found.");
            }

            await _eventDispatcher.DispatchAsync(
                new AppointmentBookedEvent(appointment.ToEventDto()));

            return ServiceResult<AppointmentReadDto>.Ok(
                appointment!.ToReadDto());
        }

        public async Task<ServiceResult<AppointmentReadDto>> RescheduleAsync(
            int appointmentId,
            AppointmentRescheduleDto dto)
        {
            var validation =
                await _validator.ValidateRescheduleAsync(
                    appointmentId,
                    dto);

            if (!validation.Success)
            {
                return validation.ToGeneric<AppointmentReadDto>();
            }

            var appointment =
                await _appointmentRepository.GetByIdAsync(
                    appointmentId);

            if (appointment == null)
            {
                return ServiceResult<AppointmentReadDto>.Fail(
                    ErrorCodes.NotFound,
                    "Appointment not found.");
            }

            if (appointment.Status is
                AppointmentStatus.Completed or
                AppointmentStatus.Cancelled or
                AppointmentStatus.NoShow)
            {
                return ServiceResult<AppointmentReadDto>.Fail(
                    ErrorCodes.Validation,
                    $"A {appointment.Status} appointment cannot be rescheduled.");
            }

            var availability =
                await ValidateBookingAvailabilityAsync(
                    dto.DoctorId,
                    dto.AppointmentDate,
                    dto.AppointmentTime,
                    dto.DurationMinutes,
                    appointmentId);

            if (!availability.Success)
            {
                return availability.ToGeneric<AppointmentReadDto>();
            }

            appointment.DoctorId = dto.DoctorId;
            appointment.PatientId = dto.PatientId;
            appointment.CustomerId = dto.CustomerId;

            appointment.AppointmentDate = dto.AppointmentDate.Date;
            appointment.AppointmentTime = dto.AppointmentTime;
            appointment.DurationMinutes = dto.DurationMinutes;

            appointment.Status = AppointmentStatus.Rescheduled;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            appointment = await _appointmentRepository.GetByIdAsync(
                appointment.AppointmentId);

            if (appointment == null)
            {
                throw new Exception("Appointment not found.");
            }

            await _eventDispatcher.DispatchAsync(
                new AppointmentRescheduledEvent(appointment.ToEventDto()));

            return ServiceResult<AppointmentReadDto>.Ok(
                appointment!.ToReadDto());
        }

        public async Task<ServiceResult> CancelAsync(
            int appointmentId)
        {
            var appointment =
                await _appointmentRepository.GetByIdAsync(
                    appointmentId);

            if (appointment == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "Appointment not found.");
            }

            if (appointment.Status is
                AppointmentStatus.Completed or
                AppointmentStatus.Cancelled)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    $"A {appointment.Status} appointment cannot be cancelled.");
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            await _eventDispatcher.DispatchAsync(
                new AppointmentCancelledEvent(appointment.ToEventDto()));

            return ServiceResult.Ok();
        }

        private async Task<ServiceResult> ValidateBookingAvailabilityAsync(
            Guid doctorId,
            DateTime appointmentDate,
            TimeSpan appointmentTime,
            int durationMinutes,
            int? ignoredAppointmentId = null)
        {
            var doctor =
                await _doctorRepository.GetByIdAsync(doctorId);

            if (doctor == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "Doctor not found.");
            }

            var schedule =
                await _doctorScheduleRepository.GetScheduleByDoctorAndDayAsync(
                    doctorId,
                    appointmentDate.DayOfWeek);

            var appointments =
                await _appointmentRepository.GetDoctorAppointmentsByDateAsync(
                    doctorId,
                    appointmentDate,
                    ignoredAppointmentId);

            return _availabilityValidator.ValidateAvailability(
                doctor,
                schedule,
                appointments,
                ignoredAppointmentId,
                appointmentDate,
                appointmentTime,
                durationMinutes);
        }
    }
}