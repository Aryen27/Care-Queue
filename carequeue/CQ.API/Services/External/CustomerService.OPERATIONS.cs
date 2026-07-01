using carequeue.CQ.API.DTOs.CustomerDTO;
using carequeue.CQ.API.Mappers;
using carequeue.CQ.API.Models.DTOs.Common;

namespace carequeue.CQ.API.Services
{
    public partial class CustomerService
    {
        public async Task<ServiceResult<CustomerPatientsReadDto>>
            GetPatientsAsync(int customerId)
        {
            var customer =
                await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                return ServiceResult<CustomerPatientsReadDto>.Fail(
                    ErrorCodes.NotFound,
                    "Customer not found.");
            }

            var patients =
                await _customerRepository.GetPatientsByCustomerIdAsync(customerId);

            customer.Patients = patients.ToList();

            return ServiceResult<CustomerPatientsReadDto>
                .Ok(customer.ToPatientsReadDto());
        }
    }
}