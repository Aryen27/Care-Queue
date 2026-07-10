using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.DTOs.AppointmentDTO
{
    public class AppointmentRescheduleDto
    {
        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [Required]
        [Range(1, 480)]
        public int DurationMinutes { get; set; } = 30;
    }
}