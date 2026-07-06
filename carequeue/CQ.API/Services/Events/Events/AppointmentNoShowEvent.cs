using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Services.Events.Interfaces;

namespace carequeue.CQ.API.Services.Events.Events
{
    public class AppointmentNoShowEvent : IAppointmentEvent
    {
        public AppointmentNoShowEvent(Appointment appointment)
        {
            Appointment = appointment;
            OccurredAt = DateTime.UtcNow;
        }

        public Appointment Appointment { get; }

        public DateTime OccurredAt { get; }
    }
}