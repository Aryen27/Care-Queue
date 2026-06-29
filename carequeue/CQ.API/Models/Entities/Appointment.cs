using carequeue.CQ.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carequeue.CQ.API.Models.Entites
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentId { get; set; }

        [Required]
        public int? HospitalId { get; set; }

        [ForeignKey(nameof(HospitalId))]
        public Hospital? Hospital { get; set; } = null!;

        [Required]
        public Guid? PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public Patient? Patient { get; set; } = null!;

        [Required]
        public Guid? DoctorId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Doctor? Doctor { get; set; } = null!;

        [Required]
        public int? CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; } = null!;

        [Required]
        [Column(TypeName = "date")]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan AppointmentTime { get; set; }

        [Required]
        [StringLength(20)]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled; // Scheduled, Completed, Cancelled, No Show, Rescheduled

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
