using carequeue.CQ.API.DTOs.CustomerDTO;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Repositories.Interfaces;

namespace carequeue.CQ.API.Services.Validators
{
    public class CustomerValidator
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerValidator(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<ServiceResult> ValidateCreateAsync(CustomerCreateDto dto)
        {
            var errors = new List<ValidationError>();

            if (await _customerRepository.EmailExistsAsync(dto.Email))
            {
                errors.Add(new ValidationError
                {
                    Property = nameof(dto.Email),
                    Message = "Email already exists."
                });
            }

            if (errors.Any())
            {
                return ServiceResult.Validation(errors);
            }

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> ValidateUpdateAsync(
            int customerId,
            CustomerUpdateDto dto)
        {
            var errors = new List<ValidationError>();

            var existingCustomer =
                await _customerRepository.GetByIdAsync(customerId);

            if (existingCustomer == null)
            {
                errors.Add(new ValidationError
                {
                    Property = "Customer",
                    Message = "Customer not found."
                });

                return ServiceResult.Validation(errors);
            }

            if (!string.Equals(
                    existingCustomer.Email,
                    dto.Email,
                    StringComparison.OrdinalIgnoreCase))
            {
                if (await _customerRepository.EmailExistsAsync(dto.Email))
                {
                    errors.Add(new ValidationError
                    {
                        Property = nameof(dto.Email),
                        Message = "Email already exists."
                    });
                }
            }

            if (errors.Any())
            {
                return ServiceResult.Validation(errors);
            }

            return ServiceResult.Ok();
        }
    }
}