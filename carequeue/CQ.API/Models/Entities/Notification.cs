using carequeue.CQ.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carequeue.CQ.API.Models.Entities
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
        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; } = null!;

        [Required]
        public Guid PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public Patient Patient { get; set; } = null!;

        public int? AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public Appointment? Appointment { get; set; }

        [Required]
        public int TemplateId { get; set; }

        [ForeignKey(nameof(TemplateId))]
        public NotificationTemplate Template { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public NotificationChannel Channel { get; set; } = NotificationChannel.Email;

        [Required]
        [StringLength(20)]
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        [Required]
        [StringLength(250)]
        public string Recipient { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Subject { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime? SentAt { get; set; }

        [StringLength(500)]
        public string? FailureReason { get; set; }

        public int RetryCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? NextRetryAt { get; set; }
    }
}