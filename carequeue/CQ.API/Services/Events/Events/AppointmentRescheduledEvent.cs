using carequeue.CQ.API.Models.DTOs.EventDTO;
using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Services.Events.Interfaces;

namespace carequeue.CQ.API.Services.Events.Events
{
    public class AppointmentRescheduledEvent : IAppointmentEvent
    {
        public AppointmentRescheduledEvent(AppointmentEventDto appointment)
        {
            Appointment = appointment;
            OccurredAt = DateTime.UtcNow;
        }

        public AppointmentEventDto Appointment { get; }
        public DateTime OccurredAt { get; }
    }
}