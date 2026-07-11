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

        public async Task<ServiceResult<IEnumerable<DoctorAvailabilityDto>>> GetAvailableDoctorsAsync(
            DoctorAvailabilityRequestDto request)
        {
            var doctors = (await _doctorRepository
                .GetDoctorsByHospitalAndSpecializationAsync(
                    request.HospitalId,
                    request.Specialization))
                .Where(d => d.IsActive && d.IsAvailable)
                .ToList();

            if (!doctors.Any())
            {
                return ServiceResult<IEnumerable<DoctorAvailabilityDto>>.Ok([]);
            }

            var doctorIds = doctors.Select(d => d.DoctorId).ToList();

            // 1. Fetching data in bulk from repositories
            var schedulesList = await _doctorScheduleRepository
                .GetSchedulesByDoctorsAndDayBulkAsync(
                    doctorIds,
                    request.AppointmentDate.DayOfWeek);

            var appointmentsList = await _appointmentRepository
                .GetDoctorAppointmentsByDateBulkAsync(
                    doctorIds,
                    request.AppointmentDate);

            // 2. Optimization: Convert bulk results to lookups for O(1) inside the loop
            var schedulesLookup = schedulesList.ToDictionary(s => s.DoctorId);
            var appointmentsLookup = appointmentsList.ToLookup(a => a.DoctorId);

            var availableDoctors = new List<DoctorAvailabilityDto>();

            foreach (var doctor in doctors)
            {
                // O(1) dictionary lookup instead of FirstOrDefault()
                schedulesLookup.TryGetValue(doctor.DoctorId, out var schedule);

                if (schedule == null) continue;

                // O(1) lookup instead of Where()
                var doctorAppointments = appointmentsLookup[doctor.DoctorId];

                var availability = _availabilityValidator.ValidateAvailability(
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
                        AppointmentEndTime = request.AppointmentTime.Add(TimeSpan.FromMinutes(request.DurationMinutes)),
                        DurationMinutes = request.DurationMinutes,
                        SlotDurationMinutes = schedule.SlotDurationMinutes
                    });
            }

            return ServiceResult<IEnumerable<DoctorAvailabilityDto>>.Ok(availableDoctors);
        }
    }
}