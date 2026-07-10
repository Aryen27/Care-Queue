using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.DTOs.DoctorDTO;

namespace carequeue.CQ.API.Services.External
{
    public partial class AppointmentService
    {
        public async Task<ServiceResult<bool>> IsDoctorAvailableAsync(
            Guid doctorId,
            DateTime appointmentDate,
            TimeSpan appointmentTime,
            int durationMinutes)
        {
            var availability =
                await ValidateBookingAvailabilityAsync(
                    doctorId,
                    appointmentDate,
                    appointmentTime,
                    durationMinutes);

            return ServiceResult<bool>.Ok(availability.Success);
        }

        public async Task<ServiceResult<IEnumerable<DoctorAvailabilityDto>>>
            GetAvailableDoctorsAsync(
                DoctorAvailabilityRequestDto request)
        {
            var doctors =
                (await _doctorRepository
                    .GetDoctorsByHospitalAndSpecializationAsync(
                        request.HospitalId,
                        request.Specialization))
                .Where(d => d.IsActive && d.IsAvailable)
                .ToList();

            if (!doctors.Any())
            {
                return ServiceResult<IEnumerable<DoctorAvailabilityDto>>
                    .Ok([]);
            }

            var doctorIds =
                doctors.Select(d => d.DoctorId);

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

            var availableDoctors =
                new List<DoctorAvailabilityDto>();

            foreach (var doctor in doctors)
            {
                var schedule =
                    schedules.FirstOrDefault(s =>
                        s.DoctorId == doctor.DoctorId);

                var doctorAppointments =
                    appointments.Where(a =>
                        a.DoctorId == doctor.DoctorId);

                var availability =
                    _availabilityValidator.ValidateAvailability(
                        doctor,
                        schedule,
                        doctorAppointments,
                        null,
                        request.AppointmentDate,
                        request.AppointmentTime,
                        request.DurationMinutes);

                if (!availability.Success)
                {
                    continue;
                }

                availableDoctors.Add(
                    new DoctorAvailabilityDto
                    {
                        DoctorId = doctor.DoctorId,
                        DoctorName = doctor.Name,
                        Specialization = doctor.Specialization,
                        ConsultationFee = doctor.ConsultationFee,

                        IsAvailable = true,

                        AppointmentDate = request.AppointmentDate,

                        AppointmentTime = request.AppointmentTime,

                        AppointmentEndTime =
                            request.AppointmentTime.Add(
                                TimeSpan.FromMinutes(request.DurationMinutes)),

                        DurationMinutes = request.DurationMinutes,

                        SlotDurationMinutes = schedule!.SlotDurationMinutes
                    });
            }

            return ServiceResult<IEnumerable<DoctorAvailabilityDto>>
                .Ok(availableDoctors);
        }
    }
}