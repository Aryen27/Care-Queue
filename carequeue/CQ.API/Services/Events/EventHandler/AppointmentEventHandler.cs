using carequeue.CQ.API.Services.Events.Interfaces;

namespace carequeue.CQ.API.Services.Events.EventHandler
{
    public abstract class AppointmentEventHandler
            : IAppointmentEventHandler
    {
        public abstract Task HandleAsync(
            IAppointmentEvent appointmentEvent);
    }
}
