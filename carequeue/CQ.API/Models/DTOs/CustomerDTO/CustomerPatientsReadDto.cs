namespace carequeue.CQ.API.DTOs.CustomerDTO
{
    public class CustomerPatientsReadDto
    {
        public int CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public List<CustomerPatientDto> Patients { get; set; } = new();
    }
}