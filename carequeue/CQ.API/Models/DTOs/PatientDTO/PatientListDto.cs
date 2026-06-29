namespace carequeue.CQ.API.DTOs.PatientDTO
{
    public class PatientListDto
    {
        public Guid PatientId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public DateTime DOB { get; set; }
    }
}