using carequeue.CQ.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.DTOs.UserDTO
{
    public class UserCreateDto
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
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.Nurse;
    }
}