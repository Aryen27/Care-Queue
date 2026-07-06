using carequeue.CQ.API.Services.Events.Interfaces;

namespace carequeue.CQ.API.Services.Events.EventHandler
{
    public class AppointmentEventDispatcher : IAppointmentEventDispatcher
    {
        private readonly IEnumerable<IAppointmentEventHandler> _handlers;

        public AppointmentEventDispatcher(
            IEnumerable<IAppointmentEventHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task DispatchAsync(
            IAppointmentEvent appointmentEvent)
        {
            foreach (var handler in _handlers)
            {
                await handler.HandleAsync(appointmentEvent);
            }
        }
    }
}