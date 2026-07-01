using carequeue.CQ.API.DTOs.DoctorDTO;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.Entites;
using carequeue.CQ.API.Repositories.Interfaces;
using carequeue.CQ.API.Services.Validators;

namespace carequeue.CQ.API.Services.External
{
    public class DoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly DoctorValidator _validator;

        public DoctorService(
            IDoctorRepository doctorRepository,
            DoctorValidator validator)
        {
            _doctorRepository = doctorRepository;
            _validator = validator;
        }

        public async Task<ServiceResult<IEnumerable<DoctorListDto>>> GetAllAsync()
        {
            var doctors = await _doctorRepository.GetAllAsync();

            var result = doctors.Select(d => new DoctorListDto
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                Specialization = d.Specialization,
                ConsultationFee = d.ConsultationFee,
                IsAvailable = d.IsAvailable,
                IsActive = d.IsActive
            });

            return ServiceResult<IEnumerable<DoctorListDto>>.Ok(result);
        }

        public async Task<ServiceResult<IEnumerable<DoctorListDto>>> GetDoctorsByHospitalIdAsync(int hospitalId)
        {
            var doctors = await _doctorRepository.GetDoctorsByHospitalIdAsync(hospitalId);

            var result = doctors.Select(d => new DoctorListDto
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                Specialization = d.Specialization,
                ConsultationFee = d.ConsultationFee,
                IsAvailable = d.IsAvailable,
                IsActive = d.IsActive
            });

            return ServiceResult<IEnumerable<DoctorListDto>>.Ok(result);
        }

        public async Task<ServiceResult<DoctorReadDto>> GetByIdAsync(Guid doctorId)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);

            if (doctor == null)
            {
                return ServiceResult<DoctorReadDto>.Fail(
                    ErrorCodes.NotFound,
                    "Doctor not found.");
            }

            return ServiceResult<DoctorReadDto>.Ok(new DoctorReadDto
            {
                DoctorId = doctor.DoctorId,
                HospitalId = doctor.HospitalId,
                HospitalName = doctor.Hospital.HospitalName,
                Name = doctor.Name,
                Specialization = doctor.Specialization,
                ConsultationFee = doctor.ConsultationFee,
                Email = doctor.Email,
                Phone = doctor.Phone,
                IsAvailable = doctor.IsAvailable,
                IsActive = doctor.IsActive
            });
        }

        public async Task<ServiceResult<DoctorReadDto>> CreateAsync(DoctorCreateDto dto)
        {
            var validation = await _validator.ValidateCreateAsync(dto);

            if (!validation.Success)
            {
                return validation.ToGeneric<DoctorReadDto>();
            }

            var doctor = new Doctor
            {
                HospitalId = dto.HospitalId,
                Name = dto.Name,
                Specialization = dto.Specialization,
                ConsultationFee = dto.ConsultationFee,
                Email = dto.Email,
                Phone = dto.Phone,
                IsAvailable = dto.IsAvailable,
                IsActive = true
            };

            await _doctorRepository.AddAsync(doctor);
            await _doctorRepository.SaveChangesAsync();

            doctor = await _doctorRepository.GetByIdAsync(doctor.DoctorId);

            return ServiceResult<DoctorReadDto>.Ok(new DoctorReadDto
            {
                DoctorId = doctor!.DoctorId,
                HospitalId = doctor.HospitalId,
                HospitalName = doctor.Hospital.HospitalName,
                Name = doctor.Name,
                Specialization = doctor.Specialization,
                ConsultationFee = doctor.ConsultationFee,
                Email = doctor.Email,
                Phone = doctor.Phone,
                IsAvailable = doctor.IsAvailable,
                IsActive = doctor.IsActive
            });
        }

        public async Task<ServiceResult> UpdateAsync(Guid doctorId, DoctorUpdateDto dto)
        {
            var validation = await _validator.ValidateUpdateAsync(doctorId, dto);

            if (!validation.Success)
            {
                return validation;
            }

            var doctor = await _doctorRepository.GetByIdAsync(doctorId);

            if (doctor == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "Doctor not found.");
            }

            doctor.HospitalId = dto.HospitalId;
            doctor.Name = dto.Name;
            doctor.Specialization = dto.Specialization;
            doctor.ConsultationFee = dto.ConsultationFee;
            doctor.Email = dto.Email;
            doctor.Phone = dto.Phone;
            doctor.IsAvailable = dto.IsAvailable;
            doctor.IsActive = dto.IsActive;

            await _doctorRepository.UpdateAsync(doctor);
            await _doctorRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(Guid doctorId)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);

            if (doctor == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "Doctor not found.");
            }

            await _doctorRepository.DeleteAsync(doctor);
            await _doctorRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }
    }
}
