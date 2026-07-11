using carequeue.CQ.API.Repositories.Interfaces;
using carequeue.CQ.API.Services.Events.Interfaces;
using carequeue.CQ.API.Services.Validators;

namespace carequeue.CQ.API.Services.External
{
    public partial class AppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;

        private readonly AppointmentValidator _validator;
        private readonly AppointmentAvailabilityValidator _availabilityValidator;

        private readonly IAppointmentEventDispatcher _eventDispatcher;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            IDoctorScheduleRepository doctorScheduleRepository,
            AppointmentValidator validator,
            AppointmentAvailabilityValidator availabilityValidator,
            IAppointmentEventDispatcher eventDispatcher)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _doctorScheduleRepository = doctorScheduleRepository;

            _validator = validator;
            _availabilityValidator = availabilityValidator;

            _eventDispatcher = eventDispatcher;
        }
    }
}