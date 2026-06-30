namespace carequeue.CQ.API.Models.DTOs.Common
{
    public class Error
    {
        public string Code { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;

        public List<ValidationError> ValidationErrors { get; init; } = [];
    }
}
