namespace carequeue.CQ.API.DTOs.PatientDTO
{
    public class PatientReadDto
    {
        public Guid PatientId { get; set; }

        public int HospitalId { get; set; }

        public string HospitalName { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string Phone { get; set; } = string.Empty;

        public DateTime DOB { get; set; }

        public string Gender { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}