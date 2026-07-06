using carequeue.CQ.API.Models.Entities;

namespace carequeue.CQ.API.Services.Events.Interfaces
{
    public interface IAppointmentEvent
    {
        Appointment Appointment { get; }

        DateTime OccurredAt { get; }
    }
}