using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carequeue.CQ.API.Models.Entites
{
    [Table("Patients")]
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientId { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [ForeignKey(nameof(HospitalId))]
        public Hospital Hospital { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string PhoneExtension { get; set; } = string.Empty;

        [StringLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime DOB { get; set; }

        [Required]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
