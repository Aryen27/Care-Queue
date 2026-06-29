using carequeue.CQ.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carequeue.CQ.API.Models.Entites
{
    [Table("OtpVerifications")]
    public class OtpVerification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OtpId { get; set; }

        [Required]
        public int? CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; } = null!;

        [Required]
        [StringLength(256)]
        public string OtpCodeHash { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public OtpPurpose Purpose { get; set; } = OtpPurpose.EmailVerification;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public DateTime? UsedAt { get; set; }

        public int Attempts { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
