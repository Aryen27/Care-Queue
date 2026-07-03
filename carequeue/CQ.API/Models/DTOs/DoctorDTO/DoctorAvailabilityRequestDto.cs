using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.Models.DTOs.DoctorDTO
{
    public class DoctorAvailabilityRequestDto
    {
        [Required]
        public int HospitalId { get; set; }

        [Required]
        public string Specialization { get; set; } = string.Empty;

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [Required]
        public int DurationMinutes { get; set; }
    }
}
