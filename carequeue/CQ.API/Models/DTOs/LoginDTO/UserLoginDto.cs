using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.Models.DTOs.LoginDTO
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
