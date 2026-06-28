using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carequeue.CQ.API.Models.Entites
{
    [Table("Hospitals")]
    public class Hospital
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HospitalId { get; set; }

        [Required]
        [StringLength(200)]
        public string HospitalName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(20)]
        [Phone]
        public string? Phone { get; set; }

        [StringLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
