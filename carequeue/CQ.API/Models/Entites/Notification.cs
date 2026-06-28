using carequeue.CQ.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carequeue.CQ.API.Models.Entites
{
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [ForeignKey(nameof(HospitalId))]
        public Hospital Hospital { get; set; } = null!;

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Required]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public Patient Patient { get; set; } = null!;

        public int? AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public Appointment? Appointment { get; set; }

        [Required]
        public int TemplateId { get; set; }

        [Required]
        [StringLength(20)]
        public NotificationChannel Channel { get; set; } = NotificationChannel.Email;

        [Required]
        [StringLength(20)]
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        [Required]
        [StringLength(250)]
        public string Recipient { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime? SentAt { get; set; }

        [StringLength(500)]
        public string? FailureReason { get; set; }

        public int RetryCount { get; set; }
    }
}
