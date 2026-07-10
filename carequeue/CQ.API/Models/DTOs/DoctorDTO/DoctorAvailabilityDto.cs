namespace carequeue.CQ.API.Models.DTOs.DoctorDTO
{
    public class DoctorAvailabilityDto
    {
        public Guid DoctorId { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public decimal ConsultationFee { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime AppointmentDate { get; set; }

        public TimeSpan AppointmentTime { get; set; }

        public TimeSpan AppointmentEndTime { get; set; }

        public int DurationMinutes { get; set; }

        public int SlotDurationMinutes { get; set; }
    }
}