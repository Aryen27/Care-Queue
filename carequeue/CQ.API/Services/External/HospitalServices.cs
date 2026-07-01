using carequeue.CQ.API.DTOs.HospitalDTO;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Models.Entites;
using carequeue.CQ.API.Repositories.Interfaces;


namespace carequeue.CQ.API.Services.External
{
    public class HospitalServices
    {
        private readonly IHospitalRepository _hospitalRepository;

        public HospitalServices(IHospitalRepository hospitalRepository)
        {
            _hospitalRepository = hospitalRepository;
        }
        public async Task<ServiceResult<IEnumerable<HospitalListDto>>> ReadAllHospitalsAsync()
        {
            var hospitals = await _hospitalRepository.GetAllAsync();

            var dtos = hospitals.Select(h => new HospitalListDto
            {
                HospitalId = h.HospitalId,
                HospitalName = h.HospitalName,
                Phone = h.Phone,
                Email = h.Email,
                IsActive = h.IsActive
            });

            return ServiceResult<IEnumerable<HospitalListDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<HospitalReadDto>> GetHospitalByIdAsync(int id)
        {
            var hospital = await _hospitalRepository.GetByIdAsync(id);
            if (hospital == null)
            {
                return ServiceResult<HospitalReadDto>.Fail("HOSPITAL_NOT_FOUND", $"Hospital with ID {id} does not exist.");
            }

            var dto = new HospitalReadDto
            {
                HospitalId = hospital.HospitalId,
                HospitalName = hospital.HospitalName,
                Address = hospital.Address,
                Phone = hospital.Phone,
                Email = hospital.Email,
                CreatedAt = hospital.CreatedAt,
                IsActive = hospital.IsActive
            };

            return ServiceResult<HospitalReadDto>.Ok(dto);
        }

        public async Task<ServiceResult> CreateHospitalAsync(HospitalCreateDto newHospitalDto)
        {
            // Example of explicit validation handling inside the service layer
            if (string.IsNullOrWhiteSpace(newHospitalDto.HospitalName))
            {
                return ServiceResult.Validation(new List<ValidationError>
                {
                    new ValidationError { Property= "HospitalName", Message= "Name is required." }
                });
            }

            var hospitalEntity = new Hospital
            {
                HospitalName = newHospitalDto.HospitalName,
                Address = newHospitalDto.Address,
                Phone = newHospitalDto.Phone,
                Email = newHospitalDto.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _hospitalRepository.AddAsync(hospitalEntity);
            await _hospitalRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> UpdateHospitalAsync(int id, HospitalUpdateDto updateDto)
        {
            var existingHospital = await _hospitalRepository.GetByIdAsync(id);
            if (existingHospital == null)
            {
                return ServiceResult.Fail("HOSPITAL_NOT_FOUND", $"Cannot update. Hospital with ID {id} was not found.");
            }

            if (!existingHospital.IsActive && updateDto.IsActive)
            {
                return ServiceResult.Fail("INVALID_STATUS_TRANSITION", "Cannot reactivate a suspended hospital record.");
            }

            existingHospital.HospitalName = updateDto.HospitalName;
            existingHospital.Address = updateDto.Address;
            existingHospital.Phone = updateDto.Phone;
            existingHospital.Email = updateDto.Email;
            existingHospital.IsActive = updateDto.IsActive;

            await _hospitalRepository.UpdateAsync(existingHospital);
            await _hospitalRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteHospitalAsync(int id)
        {
            var hospital = await _hospitalRepository.GetByIdAsync(id);
            if (hospital == null)
            {
                return ServiceResult.Fail("HOSPITAL_NOT_FOUND", $"Cannot delete! Hospital with ID {id} was not found.");
            }

            // Check relations before deleting
            // if (await _hospitalRepository.HasActivePatientsAsync(id)) 
            // {
            //     return ServiceResult.Fail("HOSPITAL_HAS_DEPENDENCIES", "Cannot delete a hospital that currently contains active patients.");
            // }

            await _hospitalRepository.DeleteAsync(hospital);
            await _hospitalRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }
    }
}
