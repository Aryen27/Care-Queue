namespace carequeue.CQ.API.DTOs.PatientDTO
{
    public class PatientListDto
    {
        public int PatientId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string PhoneExtension { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public DateTime DOB { get; set; }
    }
}