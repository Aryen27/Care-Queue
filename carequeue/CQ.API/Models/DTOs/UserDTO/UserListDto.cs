using carequeue.CQ.API.Models.Enums;

namespace carequeue.CQ.API.DTOs.UserDTO
{
    public class UserListDto
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastLogin { get; set; }
    }
}