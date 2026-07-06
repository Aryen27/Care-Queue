using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.DTOs.DoctorDTO;
using carequeue.CQ.API.Models.Entities;
using carequeue.CQ.API.Repositories.Interfaces;
using carequeue.CQ.API.Services.Validators;

namespace carequeue.CQ.API.Services.External
{
    public partial class DoctorAvailabilityService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly DoctorAvailabilityValidator _validator;

        public DoctorAvailabilityService(
            IDoctorRepository doctorRepository,
            IDoctorScheduleRepository doctorScheduleRepository,
            IAppointmentRepository appointmentRepository,
            DoctorAvailabilityValidator validator)
        {
            _doctorRepository = doctorRepository;
            _doctorScheduleRepository = doctorScheduleRepository;
            _appointmentRepository = appointmentRepository;
            _validator = validator;
        }

        private async Task<DoctorAvailabilityContext> BuildAvailabilityContextAsync(
            DoctorAvailabilityRequestDto request)
        {
            var doctors =
        (await _doctorRepository.GetDoctorsByHospitalAndSpecializationAsync(
            request.HospitalId,
            request.Specialization))
        .ToList();

            var doctorIds = doctors
                .Select(d => d.DoctorId)
                .ToList();

            var schedules =
                await _doctorScheduleRepository
                    .GetSchedulesByDoctorsAndDayBulkAsync(
                        doctorIds,
                        request.AppointmentDate.DayOfWeek);

            var appointments =
                await _appointmentRepository
                    .GetDoctorAppointmentsByDateBulkAsync(
                        doctorIds,
                        request.AppointmentDate);

            return new DoctorAvailabilityContext
            {
                Doctors = doctors,

                Schedules = schedules.ToDictionary(
                    s => s.DoctorId),

                Appointments = appointments
                    .GroupBy(a => a.DoctorId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToList())
            };
        }

        public async Task<ServiceResult<IEnumerable<DoctorAvailabilityDto>>> GetAvailableDoctorsAsync(
            DoctorAvailabilityRequestDto request)
        {
            var validation = _validator.ValidateAvailabilityRequest(request);

            if (!validation.Success)
            {
                return validation.ToGeneric<IEnumerable<DoctorAvailabilityDto>>();
            }

            var context = await BuildAvailabilityContextAsync(request);

            var availableDoctors = new List<DoctorAvailabilityDto>();

            foreach (var doctor in context.Doctors)
            {
                context.Schedules.TryGetValue(
                    doctor.DoctorId,
                    out var schedule);

                context.Appointments.TryGetValue(
                    doctor.DoctorId,
                    out var appointments);

                appointments ??= [];

                var doctorValidation =
                    _validator.ValidateDoctorAvailability(
                        doctor,
                        schedule,
                        appointments,
                        request);

                if (!doctorValidation.Success)
                {
                    continue;
                }

                availableDoctors.Add(
                    ToAvailabilityDto(
                        doctor,
                        new[] { request.AppointmentTime }));
            }

            return ServiceResult<IEnumerable<DoctorAvailabilityDto>>
                .Ok(availableDoctors);
        }

        public async Task<ServiceResult<IEnumerable<DoctorAvailabilityDto>>> GetAlternativeAvailabilityAsync(
            DoctorAvailabilityRequestDto request)
        {
            var validation = _validator.ValidateAvailabilityRequest(request);

            if (!validation.Success)
            {
                return validation.ToGeneric<IEnumerable<DoctorAvailabilityDto>>();
            }

            var alternatives = new List<DoctorAvailabilityDto>();

            var context = await BuildAvailabilityContextAsync(request);

            foreach (var doctor in context.Doctors)
            {
                context.Schedules.TryGetValue(
                    doctor.DoctorId,
                    out var schedule);

                context.Appointments.TryGetValue(
                    doctor.DoctorId,
                    out var appointments);

                appointments ??= [];

                if (schedule == null)
                    continue;

                var slots = GetAvailableSlots(
                    schedule,
                    appointments,
                    request.DurationMinutes);

                if (!slots.Any())
                    continue;

                alternatives.Add(
                    ToAvailabilityDto(
                        doctor,
                        slots));
            }

            return ServiceResult<IEnumerable<DoctorAvailabilityDto>>
                .Ok(alternatives);
        }

        private static DoctorAvailabilityDto ToAvailabilityDto(
                        Doctor doctor,
                        IEnumerable<TimeSpan> availableSlots)
        {
            return new DoctorAvailabilityDto
            {
                DoctorId = doctor.DoctorId,
                DoctorName = doctor.Name,
                Specialization = doctor.Specialization,
                ConsultationFee = doctor.ConsultationFee,
                AvailableSlots = availableSlots
            };
        }
    }
}