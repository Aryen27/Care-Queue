namespace carequeue.CQ.API.Models.DTOs.DoctorDTO
{
    public class DoctorAvailabilityDto
    {
        public Guid DoctorId { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public decimal ConsultationFee { get; set; }

        public IEnumerable<TimeSpan> AvailableSlots { get; set; } = [];
    }
}
