using carequeue.CQ.API.DTOs.UserDTO;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace carequeue.CQ.API.Services.Validators
{
    public class UserValidator 
    {
        private readonly IUserRepository _userRepository;
        private readonly IHospitalRepository _hospitalRepository;

        public UserValidator(
            IUserRepository userRepository,
            IHospitalRepository hospitalRepository)
        {
            _userRepository = userRepository;
            _hospitalRepository = hospitalRepository;
        }

        public async Task<ServiceResult> ValidateCreateAsync(UserCreateDto dto)
        {
            var errors = new List<ValidationError>();

            if (dto.Password.Length < 8)
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(dto.Password),
                    Message = "Password must contain at least 8 characters."
                });
            }

            if (await _userRepository.EmailExistsAsync(dto.Email))
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
            int userId,
            UserUpdateDto dto)
        {
            var errors = new List<ValidationError>();

            var existingUser = await _userRepository.GetByIdAsync(userId);

            if (existingUser == null)
            {
                errors.Add(new ValidationError
                {
                    Property = "User",
                    Message = "User not found."
                });

                return ServiceResult.Validation(errors);
            }

            if (!string.Equals(existingUser.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _userRepository.EmailExistsAsync(dto.Email))
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
