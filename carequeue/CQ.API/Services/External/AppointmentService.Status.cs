using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.Enums;
using carequeue.CQ.API.Services.Events.Events;

namespace carequeue.CQ.API.Services.External
{
    public partial class AppointmentService
    {
        public async Task<ServiceResult> CompleteAsync(
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

            if (appointment.Status != AppointmentStatus.Scheduled &&
                appointment.Status != AppointmentStatus.Rescheduled)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Only scheduled appointments can be completed.");
            }

            appointment.Status = AppointmentStatus.Completed;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            await _eventDispatcher.DispatchAsync(
                new AppointmentCompletedEvent(appointment));

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> MarkNoShowAsync(
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

            if (appointment.Status != AppointmentStatus.Scheduled &&
                appointment.Status != AppointmentStatus.Rescheduled)
            {
                return ServiceResult.Fail(
                    ErrorCodes.Validation,
                    "Only scheduled appointments can be marked as No Show.");
            }

            appointment.Status = AppointmentStatus.NoShow;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            await _eventDispatcher.DispatchAsync(
                new AppointmentNoShowEvent(appointment));

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> UpdateStatusAsync(
            int appointmentId,
            AppointmentStatus status)
        {
            return status switch
            {
                AppointmentStatus.Completed =>
                    await CompleteAsync(appointmentId),

                AppointmentStatus.NoShow =>
                    await MarkNoShowAsync(appointmentId),

                AppointmentStatus.Cancelled =>
                    await CancelAsync(appointmentId),

                _ => ServiceResult.Fail(
                    ErrorCodes.Validation,
                    $"Status '{status}' cannot be updated directly.")
            };
        }
    }
}
