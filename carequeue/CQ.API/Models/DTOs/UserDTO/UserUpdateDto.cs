using carequeue.CQ.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.DTOs.UserDTO
{
    public class UserUpdateDto
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
        public UserRole Role { get; set; }

        public bool IsActive { get; set; }
    }
}