using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carequeue.CQ.API.Models.Entites
{
    [Table("Doctors")]
    public class Doctor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DoctorId { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [ForeignKey(nameof(HospitalId))]
        public Hospital Hospital { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ConsultationFee { get; set; }

        [StringLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(20)]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }

        public bool IsActive { get; set; }
    }
}
