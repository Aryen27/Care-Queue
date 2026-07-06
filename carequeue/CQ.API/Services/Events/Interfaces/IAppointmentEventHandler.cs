namespace carequeue.CQ.API.Services.Events.Interfaces
{
    public interface IAppointmentEventHandler
    {
        Task HandleAsync(IAppointmentEvent appointmentEvent);
    }
}