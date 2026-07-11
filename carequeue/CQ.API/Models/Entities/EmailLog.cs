using carequeue.CQ.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carequeue.CQ.API.Models.Entities
{
    [Table("EmailLogs")]
    public class EmailLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailLogId { get; set; }

        [Required]
        public int NotificationId { get; set; }

        [ForeignKey(nameof(NotificationId))]
        public Notification Notification { get; set; } = null!;

        [Required]
        [StringLength(250)]
        public string Recipient { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Provider { get; set; } = string.Empty;

        [Required]
        public EmailStatus Status { get; set; }

        public DateTime SentAt { get; set; }

        public DateTime? OpenedAt { get; set; }

        public DateTime? ClickedAt { get; set; }

        [StringLength(500)]
        public string? FailureReason { get; set; }
    }
}
