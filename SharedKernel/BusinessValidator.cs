﻿namespace CustomerApi.SharedKernel
{
    public abstract class BusinessValidator<T>
    {
        private readonly List<(Func<T, bool> ValidationFunc, Error ValidationError)> _syncBusinessRules = new();
        private readonly List<(Func<T, Task<bool>> ValidationFunc, Error ValidationError)> _asyncBusinessRules = new();

        protected void AddAsyncBusinessRule(Func<T, Task<bool>> validationFunc, Error validationError)
        {
            _asyncBusinessRules.Add((validationFunc, validationError));
        }

        protected void AddSyncBusinessRule(Func<T, bool> validationFunc, Error validationError)
        {
            _syncBusinessRules.Add((validationFunc, validationError));
        }

        protected async Task<List<Error>> Validate(T command)
        {
            var errors = new List<Error>();

            foreach (var (validationFunc, validationError) in _syncBusinessRules)
            {
                var isValid = validationFunc(command);
                if (!isValid)
                {
                    errors.Add(validationError);
                    return errors;
                }
            }

            foreach (var (validationFunc, validationError) in _asyncBusinessRules)
            {
                var isValid = await validationFunc(command);
                if (!isValid)
                {
                    errors.Add(validationError);
                }
            }

            return errors;
        }
    }

}
