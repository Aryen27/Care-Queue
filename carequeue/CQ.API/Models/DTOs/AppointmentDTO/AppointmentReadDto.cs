using carequeue.CQ.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.DTOs.AppointmentDTO
{
    public class AppointmentReadDto
    {
        public int AppointmentId { get; set; }
        public string HospitalName { get; set; } = string.Empty;

        public string PatientName { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }
        public string CreatedByUser { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }

        public TimeSpan AppointmentTime { get; set; }

        public TimeSpan AppointmentEndTime { get; set; }

        public int DurationMinutes { get; set; } = 30;

        public AppointmentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}