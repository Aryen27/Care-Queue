namespace carequeue.CQ.API.Models.DTOs.Common
{
    public static class ErrorCodes
    {
        public const string NotFound = "NOT_FOUND";
        public const string Validation = "VALIDATION_ERROR";
        public const string Conflict = "CONFLICT";
        public const string Unauthorized = "UNAUTHORIZED";
        public const string Forbidden = "FORBIDDEN";
        public const string ServerError = "SERVER_ERROR";
    }
}
