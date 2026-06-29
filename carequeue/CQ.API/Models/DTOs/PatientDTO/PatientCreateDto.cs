using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.DTOs.PatientDTO
{
    public class PatientCreateDto
    {
        [Required]
        public int HospitalId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [Required]
        [Phone]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;
    }
}