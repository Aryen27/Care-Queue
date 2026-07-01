namespace carequeue.CQ.API.DTOs.DoctorDTO
{
    public class DoctorReadDto
    {
        public Guid DoctorId { get; set; }

        public int HospitalId { get; set; }

        public string HospitalName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public decimal ConsultationFee { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }

        public bool IsActive { get; set; }
    }
}