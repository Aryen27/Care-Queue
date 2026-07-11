using carequeue.CQ.API.Models.DTOs.EventDTO;

namespace carequeue.CQ.API.Services.Events.Interfaces
{
    public interface IAppointmentEvent
    {
        AppointmentEventDto Appointment { get; }

        DateTime OccurredAt { get; }
    }
}