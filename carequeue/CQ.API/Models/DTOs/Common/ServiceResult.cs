namespace carequeue.CQ.API.Models.DTOs.Common
{
    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; }

        private ServiceResult(
            bool success,
            T? data,
            Error? error) : base(success, error)
        {
            Data = data;
        }

        public static ServiceResult<T> Ok(T data)
        {
            return new ServiceResult<T>(true, data, null);
        }

        public new static ServiceResult<T> Fail(
            string code,
            string message)
        {
            return new ServiceResult<T>(
                false,
                default,
                new Error
                {
                    Code = code,
                    Message = message
                });
        }

        public new static ServiceResult<T> Validation(
            List<ValidationError> errors)
        {
            return new ServiceResult<T>(
                false,
                default,
                new Error
                {
                    Code = ErrorCodes.Validation,
                    Message = "Validation failed.",
                    ValidationErrors = errors
                });
        }
    }

    public class ServiceResult
    {
        public bool Success { get; }

        public Error? Error { get; }

        protected ServiceResult(bool success, Error? error)
        {
            Success = success;
            Error = error;
        }

        public static ServiceResult Ok()
        {
            return new ServiceResult(true, null);
        }

        public static ServiceResult Fail(string code, string message)
        {
            return new ServiceResult(
                false,
                new Error
                {
                    Code = code,
                    Message = message
                });
        }

        public static ServiceResult Validation(List<ValidationError> errors)
        {
            return new ServiceResult(
                false,
                new Error
                {
                    Code = ErrorCodes.Validation,
                    Message = "Validation failed.",
                    ValidationErrors = errors
                });
        }

        public ServiceResult<T> ToGeneric<T>()
        {
            if (Success)
            {
                throw new InvalidOperationException(
                    "A successful ServiceResult cannot be converted to ServiceResult<T>. Use ServiceResult<T>.Ok() instead.");
            }

            if (Error?.ValidationErrors?.Any() == true)
            {
                return ServiceResult<T>.Validation(Error.ValidationErrors);
            }

            return ServiceResult<T>.Fail(
                Error!.Code,
                Error.Message);
        }
    }
}
