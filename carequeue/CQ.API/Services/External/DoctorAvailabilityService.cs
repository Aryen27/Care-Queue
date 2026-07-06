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

        public async Task<ServiceResult<IEnumerable<DoctorAvailabilityDto>>> GetAvailableDoctorsAsync(
            DoctorAvailabilityRequestDto request)
        {
            var validation = await _validator.ValidateAvailabilityRequestAsync(request);

            if (!validation.Success)
            {
                return validation.ToGeneric<IEnumerable<DoctorAvailabilityDto>>();
            }

            var doctors =
                await _doctorRepository.GetDoctorsByHospitalAndSpecializationAsync(
                    request.HospitalId,
                    request.Specialization);

            var availableDoctors = new List<DoctorAvailabilityDto>();

            foreach (var doctor in doctors)
            {
                var doctorValidation =
                    await _validator.ValidateDoctorAvailabilityAsync(
                        doctor.DoctorId,
                        request.AppointmentDate,
                        request.AppointmentTime,
                        request.DurationMinutes);

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
            var validation = await _validator.ValidateAvailabilityRequestAsync(request);

            if (!validation.Success)
            {
                return validation.ToGeneric<IEnumerable<DoctorAvailabilityDto>>();
            }

            var doctors =
                await _doctorRepository.GetDoctorsByHospitalAndSpecializationAsync(
                    request.HospitalId,
                    request.Specialization);

            var alternatives = new List<DoctorAvailabilityDto>();

            foreach (var doctor in doctors)
            {
                var slots =
                    await GetAvailableSlotsAsync(
                        doctor.DoctorId,
                        request.AppointmentDate,
                        request.DurationMinutes);

                if (!slots.Success || slots.Data == null || !slots.Data.Any())
                {
                    continue;
                }

                alternatives.Add(
                    ToAvailabilityDto(
                        doctor,
                        slots.Data!));
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