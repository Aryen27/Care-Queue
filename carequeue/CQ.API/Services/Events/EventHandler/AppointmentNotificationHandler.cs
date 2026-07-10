using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Models.Enums;
using carequeue.CQ.API.Services.Events.Events;
using carequeue.CQ.API.Services.Events.Interfaces;
using carequeue.CQ.API.Services.Internal;

namespace carequeue.CQ.API.Services.Events.EventHandler
{
    public class AppointmentNotificationHandler : IAppointmentEventHandler
    {
        private readonly NotificationService _notificationService;

        public AppointmentNotificationHandler(
            NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(
            IAppointmentEvent appointmentEvent)
        {
            var appointment = appointmentEvent.Appointment;

            var request = new NotificationRequest
            {
                HospitalId = appointment.HospitalId,
                CustomerId = appointment.CustomerId,
                PatientId = appointment.PatientId,
                AppointmentId = appointment.AppointmentId,

                TemplateType = GetTemplateType(appointmentEvent),

                Channel = NotificationChannel.Email,

                TemplateValues = BuildTemplateValues(appointment)
            };

            await _notificationService.CreateNotificationAsync(request);
        }

        private static NotificationTemplateType GetTemplateType(IAppointmentEvent appointmentEvent)
        {
            return appointmentEvent switch
            {
                AppointmentBookedEvent => NotificationTemplateType.Scheduled,
                AppointmentCompletedEvent => NotificationTemplateType.Completed,
                AppointmentCancelledEvent => NotificationTemplateType.Cancelled,
                AppointmentRescheduledEvent => NotificationTemplateType.Rescheduled,
                AppointmentNoShowEvent => NotificationTemplateType.NoShow,
                _ => throw new InvalidOperationException($"Unsupported event type: {appointmentEvent.GetType().Name}")
            };
        }

        private static Dictionary<string, string> BuildTemplateValues(
            Appointment appointment)
        {
            return new Dictionary<string, string>
            {
                ["CustomerName"] = appointment.Customer.Name,

                ["PatientName"] = appointment.Patient.Name,

                ["DoctorName"] = appointment.Doctor.Name,

                ["HospitalName"] = appointment.Hospital.HospitalName,

                ["AppointmentId"] =
                    appointment.AppointmentId.ToString(),

                ["AppointmentDate"] =
                    appointment.AppointmentDate.ToString("dd MMM yyyy"),

                ["AppointmentTime"] =
                    appointment.AppointmentTime.ToString(@"hh\:mm")
            };
        }
    }
}