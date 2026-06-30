using carequeue.CQ.API.DTOs.UserDTO;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.Entites;
using carequeue.CQ.API.Repositories.Interfaces;
using carequeue.CQ.API.Services.Validators;
using System.ComponentModel.DataAnnotations;

namespace carequeue.CQ.API.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserValidator _validator;

        public UserService(
            IUserRepository userRepository,
            UserValidator validator)
        {
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task<ServiceResult<IEnumerable<UserListDto>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            var result = users.Select(u => new UserListDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                LastLogin = u.LastLogin
            });

            return ServiceResult<IEnumerable<UserListDto>>.Ok(result);
        }

        public async Task<ServiceResult<IEnumerable<UserListDto>>> GetUsersByHospitalIdAsync(int hospitalId)
        {
            var users = await _userRepository.GetUsersByHospitalIdAsync(hospitalId);

            var result = users.Select(u => new UserListDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                LastLogin = u.LastLogin
            });

            return ServiceResult<IEnumerable<UserListDto>>.Ok(result);
        }

        public async Task<ServiceResult<UserReadDto>> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return ServiceResult<UserReadDto>.Fail(
                    ErrorCodes.NotFound,
                    "User not found.");
            }

            var dto = new UserReadDto
            {
                UserId = user.UserId,
                HospitalId = user.HospitalId,
                HospitalName = user.Hospital?.HospitalName ?? string.Empty,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                IsEmailVerified = user.IsEmailVerified,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin
            };

            return ServiceResult<UserReadDto>.Ok(dto);
        }

        public async Task<ServiceResult<UserReadDto>> CreateAsync(UserCreateDto dto)
        {
            var validation = await _validator.ValidateCreateAsync(dto);

            if (!validation.Success)
            {
                /*
                 * Passing list of ValidationErrors Array into ServiceResult
                 * Since this method(Service) returns ServiceResult<UserReadDto> the ServiceResult response needs to be mapped to ServiceResult<UserReadDto>
                 * by calling ServiceResult<UserReadDto>.Validation() instead of ServiceResult.Validation()
                 */
                return ServiceResult<UserReadDto>.Validation(
                    validation.Error!.ValidationErrors!);
            }

            var user = new User
            {
                HospitalId = dto.HospitalId,
                Name = dto.Name,
                Email = dto.Email,

                // TODO: Hash password before saving
                PasswordHash = dto.Password,

                Role = dto.Role,
                CreatedAt = DateTime.UtcNow,
                IsEmailVerified = false,
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            user = await _userRepository.GetByIdAsync(user.UserId);

            return ServiceResult<UserReadDto>.Ok(new UserReadDto
            {
                UserId = user!.UserId,
                HospitalId = user.HospitalId,
                HospitalName = user.Hospital?.HospitalName ?? string.Empty,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                IsEmailVerified = user.IsEmailVerified,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin
            });
        }

        public async Task<ServiceResult> UpdateAsync(int id, UserUpdateDto dto)
        {
            var validation = await _validator.ValidateUpdateAsync(id, dto);

            if (!validation.Success)
            {
                return validation;
            }

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "User not found.");
            }

            user.HospitalId = dto.HospitalId;
            user.Name = dto.Name;
            user.Email = dto.Email;
            user.Role = dto.Role;
            user.IsActive = dto.IsActive;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "User not found.");
            }

            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }
    }
}
