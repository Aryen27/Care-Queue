using carequeue.CQ.API.DTOs.AppointmentDTO;
using carequeue.CQ.API.Mappers;
using carequeue.CQ.API.Models.DTOs.Common;

namespace carequeue.CQ.API.Services.External
{
    public partial class AppointmentService
    {
        public async Task<ServiceResult<IEnumerable<AppointmentListDto>>>
            GetByHospitalAsync(int hospitalId)
        {
            var appointments =
                await _appointmentRepository
                    .GetAppointmentsByHospitalIdAsync(hospitalId);

            return ServiceResult<IEnumerable<AppointmentListDto>>
                .Ok(appointments.Select(a => a.ToListDto()));
        }

        public async Task<ServiceResult<IEnumerable<AppointmentListDto>>>
            GetByDoctorAsync(Guid doctorId)
        {
            var appointments =
                await _appointmentRepository
                    .GetAppointmentsByDoctorIdAsync(doctorId);

            return ServiceResult<IEnumerable<AppointmentListDto>>
                .Ok(appointments.Select(a => a.ToListDto()));
        }

        public async Task<ServiceResult<IEnumerable<AppointmentListDto>>>
            GetByPatientAsync(Guid patientId)
        {
            var appointments =
                await _appointmentRepository
                    .GetAppointmentsByPatientIdAsync(patientId);

            return ServiceResult<IEnumerable<AppointmentListDto>>
                .Ok(appointments.Select(a => a.ToListDto()));
        }

        public async Task<ServiceResult<IEnumerable<AppointmentListDto>>>
            GetByCustomerAsync(int customerId)
        {
            var appointments =
                await _appointmentRepository
                    .GetAppointmentsByCustomerIdAsync(customerId);

            return ServiceResult<IEnumerable<AppointmentListDto>>
                .Ok(appointments.Select(a => a.ToListDto()));
        }

        public async Task<ServiceResult<IEnumerable<AppointmentListDto>>>
            GetByDateAsync(DateTime appointmentDate)
        {
            var appointments =
                await _appointmentRepository
                    .GetAppointmentsByDateAsync(appointmentDate);

            return ServiceResult<IEnumerable<AppointmentListDto>>
                .Ok(appointments.Select(a => a.ToListDto()));
        }

        public async Task<ServiceResult<IEnumerable<AppointmentListDto>>>
            GetDoctorScheduleAsync(
                Guid doctorId,
                DateTime appointmentDate)
        {
            var appointments =
                await _appointmentRepository
                    .GetDoctorAppointmentsByDateAsync(
                        doctorId,
                        appointmentDate);

            return ServiceResult<IEnumerable<AppointmentListDto>>
                .Ok(appointments.Select(a => a.ToListDto()));
        }
    }
}