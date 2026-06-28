namespace carequeue.CQ.API.DTOs.DoctorDTO
{
    public class DoctorListDto
    {
        public Guid DoctorId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public decimal ConsultationFee { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsActive { get; set; }
    }
}