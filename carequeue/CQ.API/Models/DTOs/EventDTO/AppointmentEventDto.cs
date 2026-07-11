namespace carequeue.CQ.API.Models.DTOs.EventDTO
{
    public record AppointmentEventDto(
            int AppointmentId,
            int HospitalId,
            int CustomerId,
            Guid PatientId,
            Guid DoctorId,
            string CustomerName,
            string PatientName,
            string DoctorName,
            string HospitalName,
            DateTime AppointmentDate,
            TimeSpan AppointmentTime
        );
}
