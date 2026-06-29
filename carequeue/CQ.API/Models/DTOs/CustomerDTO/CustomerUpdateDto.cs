using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.DTOs.CustomerDTO
{
    public class CustomerUpdateDto
    {

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be 10 digits.")]
        public string Phone { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}