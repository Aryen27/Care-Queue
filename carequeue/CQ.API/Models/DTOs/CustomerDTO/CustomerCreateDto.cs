using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.DTOs.CustomerDTO
{
    public class CustomerCreateDto
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

        [Required(ErrorMessage = "Initial password is required.")]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}