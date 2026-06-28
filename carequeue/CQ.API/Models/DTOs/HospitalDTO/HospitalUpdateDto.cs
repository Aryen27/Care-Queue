using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.DTOs.HospitalDTO
{
    public class HospitalUpdateDto
    {
        [Required]
        [StringLength(200)]
        public string HospitalName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        public bool IsActive { get; set; }
    }
}