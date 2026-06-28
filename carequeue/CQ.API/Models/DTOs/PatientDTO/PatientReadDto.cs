namespace carequeue.CQ.API.DTOs.PatientDTO
{
    public class PatientReadDto
    {
        public int PatientId { get; set; }

        public int HospitalId { get; set; }

        public string HospitalName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string PhoneExtension { get; set; } = string.Empty;

        public string? Email { get; set; }

        public DateTime DOB { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}