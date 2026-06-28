namespace carequeue.CQ.API.DTOs.HospitalDTO
{
    public class HospitalListDto
    {
        public int HospitalId { get; set; }

        public string HospitalName { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public bool IsActive { get; set; }
    }
}