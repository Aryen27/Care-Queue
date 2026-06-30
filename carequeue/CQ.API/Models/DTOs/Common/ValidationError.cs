namespace carequeue.CQ.API.Models.DTOs.Common
{
    public class ValidationError
    {
        public string Property { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;
    }
}
