using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.Models.DTOs.LoginDTO
{
    public class PatientLoginDto
    {
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;
    }
}
