using carequeue.CQ.API.DTOs.DoctorDTO;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Repositories.Interfaces;

namespace carequeue.CQ.API.Services.External
{
    public class DoctorAvailabilityService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public DoctorAvailabilityService(
            IDoctorRepository doctorRepository,
            IDoctorScheduleRepository doctorScheduleRepository,
            IAppointmentRepository appointmentRepository)
        {
            _doctorRepository = doctorRepository;
            _doctorScheduleRepository = doctorScheduleRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<ServiceResult<IEnumerable<DoctorListDto>>> GetAvailableDoctorsAsync(
            int hospitalId,
            string specialization,
            DateTime appointmentDate,
            TimeSpan appointmentTime)
        {
            var doctors =
                await _doctorRepository.GetDoctorsByHospitalAndSpecializationAsync(
                    hospitalId,
                    specialization);

            var availableDoctors = new List<DoctorListDto>();

            foreach (var doctor in doctors)
            {
                var available = await IsDoctorAvailableAsync(
                    doctor.DoctorId,
                    appointmentDate,
                    appointmentTime);

                if (available.Success && available.Data)
                {
                    availableDoctors.Add(new DoctorListDto
                    {
                        DoctorId = doctor.DoctorId,
                        Name = doctor.Name,
                        Specialization = doctor.Specialization,
                        ConsultationFee = doctor.ConsultationFee,
                        IsAvailable = doctor.IsAvailable,
                        IsActive = doctor.IsActive
                    });
                }
            }

            return ServiceResult<IEnumerable<DoctorListDto>>
                .Ok(availableDoctors);
        }

        public async Task<ServiceResult<IEnumerable<DoctorListDto>>> GetAlternativeDoctorsAsync(
            int hospitalId,
            string specialization,
            DateTime appointmentDate,
            TimeSpan appointmentTime)
        {
            // Currently returns all available doctors.
            // Later this can recommend nearby time slots or rank doctors.

            return await GetAvailableDoctorsAsync(
                hospitalId,
                specialization,
                appointmentDate,
                appointmentTime);
        }

        public async Task<ServiceResult<IEnumerable<TimeSpan>>> GetAvailableSlotsAsync(
            Guid doctorId,
            DateTime appointmentDate)
        {
            var schedule =
                await _doctorScheduleRepository.GetScheduleByDoctorAndDayAsync(
                    doctorId,
                    appointmentDate.DayOfWeek);

            if (schedule == null)
            {
                return ServiceResult<IEnumerable<TimeSpan>>.Fail(
                    ErrorCodes.NotFound,
                    "Doctor does not work on the selected day.");
            }

            var bookedAppointments =
                await _appointmentRepository.GetDoctorAppointmentsByDateAsync(
                    doctorId,
                    appointmentDate);

            var bookedSlots = bookedAppointments
                .Select(a => a.AppointmentTime)
                .ToHashSet();

            var availableSlots = GenerateSlots(schedule)
                .Where(slot => !bookedSlots.Contains(slot));

            return ServiceResult<IEnumerable<TimeSpan>>
                .Ok(availableSlots);
        }

        public async Task<ServiceResult<bool>> IsDoctorAvailableAsync(
            Guid doctorId,
            DateTime appointmentDate,
            TimeSpan appointmentTime)
        {
            var schedule =
                await _doctorScheduleRepository.GetScheduleByDoctorAndDayAsync(
                    doctorId,
                    appointmentDate.DayOfWeek);

            if (schedule == null)
            {
                return ServiceResult<bool>.Ok(false);
            }

            if (!schedule.isAvailable || !schedule.IsActive)
            {
                return ServiceResult<bool>.Ok(false);
            }

            if (!IsWithinWorkingHours(schedule, appointmentTime))
            {
                return ServiceResult<bool>.Ok(false);
            }

            if (!IsValidSlot(schedule, appointmentTime))
            {
                return ServiceResult<bool>.Ok(false);
            }

            var exists =
                await _appointmentRepository.AppointmentExistsAsync(
                    doctorId,
                    appointmentDate,
                    appointmentTime);

            return ServiceResult<bool>.Ok(!exists);
        }

        private static bool IsWithinWorkingHours(
            DoctorSchedule schedule,
            TimeSpan appointmentTime)
        {
            var start = schedule.StartTime.ToTimeSpan();
            var end = schedule.EndTime.ToTimeSpan();

            return appointmentTime >= start &&
                   appointmentTime < end;
        }

        private static bool IsValidSlot(
            DoctorSchedule schedule,
            TimeSpan appointmentTime)
        {
            var start = schedule.StartTime.ToTimeSpan();

            var minutes =
                (appointmentTime - start).TotalMinutes;

            return minutes % schedule.SlotDurationMinutes == 0;
        }

        private static IEnumerable<TimeSpan> GenerateSlots(
            DoctorSchedule schedule)
        {
            var slots = new List<TimeSpan>();

            var current = schedule.StartTime.ToTimeSpan();
            var end = schedule.EndTime.ToTimeSpan();

            while (current < end)
            {
                slots.Add(current);

                current = current.Add(
                    TimeSpan.FromMinutes(schedule.SlotDurationMinutes));
            }

            return slots;
        }
    }
}