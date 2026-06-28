using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.Models.DTOs.RegistrationDTO
{
    public class PatientRegistrationDto
    {
        [Required]
        public int HospitalId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(10)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string PhoneExtension { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }
    }
}
