namespace carequeue.CQ.API.DTOs.CustomerDTO

{
    public class CustomerPatientDto
    {
        public Guid PatientId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string? Email { get; set; }

        public DateTime DOB { get; set; }

        public string Gender { get; set; } = string.Empty;
    }
}