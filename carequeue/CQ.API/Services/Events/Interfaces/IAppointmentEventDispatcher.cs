namespace carequeue.CQ.API.Services.Events.Interfaces
{
    public interface IAppointmentEventDispatcher
    {
        Task DispatchAsync(IAppointmentEvent appointmentEvent);
    }
}