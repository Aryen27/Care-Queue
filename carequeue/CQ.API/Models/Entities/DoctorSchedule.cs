using carequeue.CQ.API.Models.Entites;

public class DoctorSchedule
{
    public Guid DoctorScheduleId { get; set; }

    public Guid DoctorId { get; set; }

    public Doctor Doctor { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int SlotDurationMinutes { get; set; } = 30;

    public bool isAvailable { get; set; } = true;
    public bool IsActive { get; set; } = true;

}