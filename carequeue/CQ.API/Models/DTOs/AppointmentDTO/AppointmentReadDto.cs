using carequeue.CQ.API.Models.Enums;

namespace carequeue.CQ.API.DTOs.AppointmentDTO
{
    public class AppointmentReadDto
    {
        public int AppointmentId { get; set; }

        public int HospitalId { get; set; }
        public string HospitalName { get; set; } = string.Empty;

        public string PatientName { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }
        public string CreatedByUser { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }

        public TimeSpan AppointmentTime { get; set; }

        public AppointmentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}