namespace carequeue.CQ.API.DTOs.HospitalDTO
{
    public class HospitalReadDto
    {
        public int HospitalId { get; set; }

        public string HospitalName { get; set; } = string.Empty;

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}