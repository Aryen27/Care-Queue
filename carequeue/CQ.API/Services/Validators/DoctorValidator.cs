using carequeue.CQ.API.DTOs.DoctorDTO;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Repositories.Interfaces;

namespace carequeue.CQ.API.Services.Validators
{
    public class DoctorValidator
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IHospitalRepository _hospitalRepository;

        public DoctorValidator(
            IDoctorRepository doctorRepository,
            IHospitalRepository hospitalRepository)
        {
            _doctorRepository = doctorRepository;
            _hospitalRepository = hospitalRepository;
        }

        public async Task<ServiceResult> ValidateCreateAsync(DoctorCreateDto dto)
        {
            var errors = new List<ValidationError>();

            if (await _doctorRepository.EmailExistsAsync(dto.Email))
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(dto.Email),
                    Message = "Email already exists."
                });
            }

            if (!await _hospitalRepository.ExistsAsync(dto.HospitalId))
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(dto.HospitalId),
                    Message = "Hospital does not exist."
                });
            }

            if (errors.Any())
            {
                return ServiceResult.Validation(errors);
            }

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> ValidateUpdateAsync(
            Guid doctorId,
            DoctorUpdateDto dto)
        {
            var errors = new List<ValidationError>();

            var existingDoctor = await _doctorRepository.GetByIdAsync(doctorId);

            if (existingDoctor == null)
            {
                errors.Add(new ValidationError
                {
                    Property = "Doctor",
                    Message = "Doctor not found."
                });

                return ServiceResult.Validation(errors);
            }

            if (!string.Equals(existingDoctor.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _doctorRepository.EmailExistsAsync(dto.Email))
                {
                    errors.Add(new ValidationError
                    {
                        Property = nameof(dto.Email),
                        Message = "Email already exists."
                    });
                }
            }

            if (!await _hospitalRepository.ExistsAsync(dto.HospitalId))
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(dto.HospitalId),
                    Message = "Hospital does not exist."
                });
            }

            if (errors.Any())
            {
                return ServiceResult.Validation(errors);
            }

            return ServiceResult.Ok();
        }
    }
}
