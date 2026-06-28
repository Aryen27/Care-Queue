using carequeue.CQ.API.Models.Enums;

namespace carequeue.CQ.API.DTOs.UserDTO
{
    public class UserReadDto
    {
        public int UserId { get; set; }

        public int HospitalId { get; set; }

        public string HospitalName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public bool IsEmailVerified { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLogin { get; set; }
    }
}