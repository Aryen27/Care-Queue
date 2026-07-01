using carequeue.CQ.API.DTOs.CustomerDTO;
using carequeue.CQ.API.Models.Entities;

namespace carequeue.CQ.API.Mappers
{
    public static class CustomerMappings
    {
        public static Customer ToEntity(this CustomerCreateDto dto)
        {
            return new Customer
            {
                Name = dto.Name,
                Email = dto.Email,

                // TODO:Replace with password hashing.
                PasswordHash = dto.Password,

                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        public static CustomerListDto ToListDto(this Customer customer)
        {
            return new CustomerListDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                IsActive = customer.IsActive
            };
        }

        public static CustomerReadDto ToReadDto(this Customer customer)
        {
            return new CustomerReadDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                PatientCount = customer.Patients?.Count ?? 0
            };
        }

        public static void UpdateEntity(
            this Customer customer,
            CustomerUpdateDto dto)
        {
            customer.Name = dto.Name;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.IsActive = dto.IsActive;
        }

        public static CustomerPatientDto ToCustomerPatientDto(
            this Patient patient)
        {
            return new CustomerPatientDto
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                Phone = patient.Phone,
                Email = patient.Email,
                DOB = patient.DOB,
                Gender = patient.Gender
            };
        }

        public static CustomerPatientsReadDto ToPatientsReadDto(
            this Customer customer)
        {
            return new CustomerPatientsReadDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Patients = customer.Patients
                    .Select(p => p.ToCustomerPatientDto())
                    .ToList()
            };
        }
    }
}