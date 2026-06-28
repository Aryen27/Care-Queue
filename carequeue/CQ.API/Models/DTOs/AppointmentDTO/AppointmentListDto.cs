using carequeue.CQ.API.Models.Enums;

namespace carequeue.CQ.API.DTOs.AppointmentDTO
{
    public class AppointmentListDto
    {
        public int AppointmentId { get; set; }

        public string PatientName { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }

        public TimeSpan AppointmentTime { get; set; }

        public AppointmentStatus Status { get; set; }
    }
}