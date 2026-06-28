using carequeue.CQ.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.Models.DTOs.RegistrationDTO
{
    public class UserRegistrationDto
    {
        [Required]
        public int HospitalId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.Nurse;
    }
}
