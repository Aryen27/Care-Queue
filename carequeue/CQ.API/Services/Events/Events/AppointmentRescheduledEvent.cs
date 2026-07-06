using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Services.Events.Interfaces;

namespace carequeue.CQ.API.Services.Events.Events
{
    public class AppointmentRescheduledEvent : IAppointmentEvent
    {
        public AppointmentRescheduledEvent(Appointment appointment)
        {
            Appointment = appointment;
            OccurredAt = DateTime.UtcNow;
        }

        public Appointment Appointment { get; }

        public DateTime OccurredAt { get; }
    }
}