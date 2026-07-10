using carequeue.CQ.API.DTOs.AppointmentDTO;
using carequeue.CQ.API.Mappers;
using carequeue.CQ.API.Models.DTOs.Common;

namespace carequeue.CQ.API.Services.External
{
    public partial class AppointmentService
    {
        public async Task<ServiceResult<IEnumerable<AppointmentListDto>>> GetAllAsync()
        {
            var appointments =
                await _appointmentRepository.GetAllAsync();

            return ServiceResult<IEnumerable<AppointmentListDto>>
                .Ok(appointments.Select(a => a.ToListDto()));
        }

        public async Task<ServiceResult<AppointmentReadDto>> GetByIdAsync(
            int appointmentId)
        {
            var appointment =
                await _appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                return ServiceResult<AppointmentReadDto>.Fail(
                    ErrorCodes.NotFound,
                    "Appointment not found.");
            }

            return ServiceResult<AppointmentReadDto>
                .Ok(appointment.ToReadDto());
        }

        public async Task<ServiceResult> UpdateAsync(
            int appointmentId,
            AppointmentUpdateDto dto)
        {
            var validation =
                await _validator.ValidateUpdateAsync(
                    appointmentId,
                    dto);

            if (!validation.Success)
            {
                return validation;
            }

            var appointment =
                await _appointmentRepository.GetByIdAsync(
                    appointmentId);

            if (appointment == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "Appointment not found.");
            }

            appointment.UpdateEntity(dto);

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(
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

            await _appointmentRepository.DeleteAsync(
                appointment);

            await _appointmentRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }
    }
}