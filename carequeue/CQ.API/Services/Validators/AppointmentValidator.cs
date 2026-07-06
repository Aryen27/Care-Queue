using carequeue.CQ.API.DTOs.AppointmentDTO;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Repositories.Interfaces;

namespace carequeue.CQ.API.Services.Validators
{
    public class AppointmentValidator
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IHospitalRepository _hospitalRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ICustomerRepository _customerRepository;

        public AppointmentValidator(
            IAppointmentRepository appointmentRepository,
            IHospitalRepository hospitalRepository,
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository,
            ICustomerRepository customerRepository)
        {
            _appointmentRepository = appointmentRepository;
            _hospitalRepository = hospitalRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _customerRepository = customerRepository;
        }

        public async Task<ServiceResult> ValidateCreateAsync(
            AppointmentCreateDto dto)
        {
            var errors = new List<ValidationError>();

            await ValidateCommonAsync(
                dto.HospitalId,
                dto.DoctorId,
                dto.PatientId,
                dto.CustomerId,
                dto.AppointmentDate,
                dto.DurationMinutes,
                errors);

            if (errors.Any())
            {
                return ServiceResult.Validation(errors);
            }

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> ValidateUpdateAsync(
            int appointmentId,
            AppointmentUpdateDto dto)
        {
            var errors = new List<ValidationError>();

            var appointment =
                await _appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                errors.Add(new ValidationError
                {
                    Property = "Appointment",
                    Message = "Appointment not found."
                });

                return ServiceResult.Validation(errors);
            }

            await ValidateCommonAsync(
                dto.HospitalId,
                dto.DoctorId,
                dto.PatientId,
                dto.CustomerId,
                dto.AppointmentDate,
                dto.DurationMinutes,
                errors);

            if (errors.Any())
            {
                return ServiceResult.Validation(errors);
            }

            return ServiceResult.Ok();
        }

        private async Task ValidateCommonAsync(
            int hospitalId,
            Guid doctorId,
            Guid patientId,
            int customerId,
            DateTime appointmentDate,
            int durationMinutes,
            List<ValidationError> errors)
        {
            var hospital =
                await _hospitalRepository.GetByIdAsync(hospitalId);

            if (hospital == null)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(hospitalId),
                    Message = "Hospital does not exist."
                });
            }

            var doctor =
                await _doctorRepository.GetByIdAsync(doctorId);

            if (doctor == null)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(doctorId),
                    Message = "Doctor does not exist."
                });
            }

            var patient =
                await _patientRepository.GetByIdAsync(patientId);

            if (patient == null)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(patientId),
                    Message = "Patient does not exist."
                });
            }

            var customer =
                await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(customerId),
                    Message = "Customer does not exist."
                });
            }

            if (doctor != null &&
                doctor.HospitalId != hospitalId)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(doctorId),
                    Message = "Doctor does not belong to the selected hospital."
                });
            }

            if (patient != null &&
                patient.HospitalId != hospitalId)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(patientId),
                    Message = "Patient does not belong to the selected hospital."
                });
            }

            if (patient != null &&
                patient.CustomerId != customerId)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(customerId),
                    Message = "Patient does not belong to the selected customer."
                });
            }

            if (appointmentDate.Date < DateTime.Today)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(appointmentDate),
                    Message = "Appointment date cannot be in the past."
                });
            }

            if (durationMinutes <= 0)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(durationMinutes),
                    Message = "Appointment duration must be greater than zero."
                });
            }
        }
    }
}