using carequeue.CQ.API.Models.Enums;

public class NotificationRequest
{
    public int HospitalId { get; set; }
    public int CustomerId { get; set; }
    public Guid PatientId { get; set; }
    public int? AppointmentId { get; set; }

    public NotificationTemplateType TemplateType { get; set; }

    public NotificationChannel Channel { get; set; } = NotificationChannel.Email;

    public Dictionary<string, string> TemplateValues { get; set; }
}