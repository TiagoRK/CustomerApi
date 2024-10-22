namespace CustomerApi.SharedKernel
{
    public class Result<TValue, TError>
    {
        public readonly TValue? Value;
        public readonly List<TError>? Errors;

        private readonly bool _isSuccess;

        private Result(TValue value)
        {
            _isSuccess = true;
            Value = value;
            Errors = null;
        }

        private Result(List<TError> errors)
        {
            _isSuccess = false;
            Value = default;
            Errors = errors;
        }

        public static implicit operator Result<TValue, TError>(TValue value) => new(value);

        public static implicit operator Result<TValue, TError>(TError error) => new([error]);

        public static implicit operator Result<TValue, TError>(List<TError> errors) => new(errors);

        public bool IsSuccess => _isSuccess;
    }
}
