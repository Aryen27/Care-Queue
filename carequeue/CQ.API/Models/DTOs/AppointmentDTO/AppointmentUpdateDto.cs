using carequeue.CQ.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.DTOs.AppointmentDTO
{
    public class AppointmentUpdateDto
    {
        [Required]
        public int HospitalId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; }
    }
}