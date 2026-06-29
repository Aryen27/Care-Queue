using System.Collections.Generic;

namespace carequeue.CQ.API.DTOs.CustomerDTO
{
    public class CustomerReadDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Return a list of basic details or counts of their managed patients
        public int PatientCount { get; set; }
    }
}