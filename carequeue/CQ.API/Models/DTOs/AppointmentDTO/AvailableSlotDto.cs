namespace carequeue.CQ.API.Models.DTOs.AppointmentDTO
{
    public class AvailableSlotDto
    {
        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsAvailable { get; set; }
    }
}
