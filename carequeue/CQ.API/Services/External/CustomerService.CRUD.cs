using carequeue.CQ.API.DTOs.CustomerDTO;
using carequeue.CQ.API.Mappers;
using carequeue.CQ.API.Models.DTOs.Common;
using carequeue.CQ.API.Repositories.Interfaces;
using carequeue.CQ.API.Services.Validators;

namespace carequeue.CQ.API.Services
{
    public partial class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly CustomerValidator _validator;

        public CustomerService(
            ICustomerRepository customerRepository,
            CustomerValidator validator)
        {
            _customerRepository = customerRepository;
            _validator = validator;
        }

        public async Task<ServiceResult<IEnumerable<CustomerListDto>>> GetAllAsync()
        {
            var customers = await _customerRepository.GetAllAsync();

            return ServiceResult<IEnumerable<CustomerListDto>>
                .Ok(customers.Select(c => c.ToListDto()));
        }

        public async Task<ServiceResult<CustomerReadDto>> GetByIdAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                return ServiceResult<CustomerReadDto>.Fail(
                    ErrorCodes.NotFound,
                    "Customer not found.");
            }

            return ServiceResult<CustomerReadDto>.Ok(customer.ToReadDto());
        }

        public async Task<ServiceResult<CustomerReadDto>> CreateAsync(CustomerCreateDto dto)
        {
            var validation = await _validator.ValidateCreateAsync(dto);

            if (!validation.Success)
            {
                return validation.ToGeneric<CustomerReadDto>();
            }

            var customer = dto.ToEntity();

            // TODO: Replace PasswordHash assignment with a secure password hashing implementation.
            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return ServiceResult<CustomerReadDto>.Ok(customer.ToReadDto());
        }

        public async Task<ServiceResult> UpdateAsync(
            int customerId,
            CustomerUpdateDto dto)
        {
            var validation =
                await _validator.ValidateUpdateAsync(customerId, dto);

            if (!validation.Success)
            {
                return validation;
            }

            var customer =
                await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "Customer not found.");
            }

            customer.UpdateEntity(dto);

            await _customerRepository.UpdateAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> DeleteAsync(int customerId)
        {
            var customer =
                await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                return ServiceResult.Fail(
                    ErrorCodes.NotFound,
                    "Customer not found.");
            }

            await _customerRepository.DeleteAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return ServiceResult.Ok();
        }
    }
}