using carequeue.CQ.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.DTOs.AppointmentDTO
{
    public class AppointmentListDto
    {
        public int AppointmentId { get; set; }
        public string HospitalName { get; set; } = string.Empty;

        public string PatientName { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }

        public TimeSpan AppointmentTime { get; set; }

        public int DurationMinutes { get; set; } = 30;

        public AppointmentStatus Status { get; set; }
    }
}