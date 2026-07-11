using carequeue.CQ.API.DTOs.AppointmentDTO;
using carequeue.CQ.API.Models.DTOs.EventDTO;
using carequeue.CQ.API.Models.Entities;

namespace carequeue.CQ.API.Mappers
{
    public static class AppointmentMapper
    {
        public static Appointment ToEntity(
            this AppointmentCreateDto dto,
            int customerId)
        {
            return new Appointment
            {
                HospitalId = dto.HospitalId,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                CustomerId = customerId,
                AppointmentDate = dto.AppointmentDate.Date,
                AppointmentTime = dto.AppointmentTime,
                DurationMinutes = dto.DurationMinutes,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static TimeSpan GetEndTime(this Appointment appointment)
        {
            return appointment.AppointmentTime.Add(
                TimeSpan.FromMinutes(appointment.DurationMinutes));
        }

        public static AppointmentReadDto ToReadDto(
            this Appointment appointment)
        {
            return new AppointmentReadDto
            {
                AppointmentId = appointment.AppointmentId,

                HospitalName = appointment.Hospital.HospitalName,

                PatientName = appointment.Patient.Name,

                DoctorName = appointment.Doctor.Name,

                CustomerId = appointment.Customer.CustomerId,
                CustomerName = appointment.Customer.Name,

                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,

                DurationMinutes = appointment.DurationMinutes,

                AppointmentEndTime = appointment.GetEndTime(),

                Status = appointment.Status,

                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt
            };
        }

        public static AppointmentListDto ToListDto(
            this Appointment appointment)
        {
            return new AppointmentListDto
            {
                AppointmentId = appointment.AppointmentId,

                HospitalName = appointment.Hospital.HospitalName,

                PatientName = appointment.Patient.Name,

                DoctorName = appointment.Doctor.Name,

                AppointmentDate = appointment.AppointmentDate,

                AppointmentTime = appointment.AppointmentTime,

                DurationMinutes = appointment.DurationMinutes,

                Status = appointment.Status
            };
        }

        public static void UpdateEntity(
            this Appointment appointment,
            AppointmentUpdateDto dto)
        {
            appointment.HospitalId = dto.HospitalId;
            appointment.PatientId = dto.PatientId;
            appointment.DoctorId = dto.DoctorId;
            appointment.CustomerId = dto.CustomerId;

            appointment.AppointmentDate = dto.AppointmentDate.Date;
            appointment.AppointmentTime = dto.AppointmentTime;

            appointment.DurationMinutes = dto.DurationMinutes;

            appointment.Status = dto.Status;

            appointment.UpdatedAt = DateTime.UtcNow;
        }

        public static AppointmentEventDto ToEventDto(this Appointment appointment)
        {
            return new AppointmentEventDto(
                appointment.AppointmentId,
                appointment.HospitalId,
                appointment.CustomerId,
                appointment.PatientId,
                appointment.DoctorId,
                appointment.Customer?.Name ?? string.Empty,
                appointment.Patient?.Name ?? string.Empty,
                appointment.Doctor?.Name ?? string.Empty,
                appointment.Hospital?.HospitalName ?? string.Empty,
                appointment.AppointmentDate,
                appointment.AppointmentTime
            );
        }
    }
}