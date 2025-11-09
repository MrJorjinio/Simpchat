using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Shared.Models
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }

        protected internal Result(bool isSuccess, Error error, IReadOnlyDictionary<string, string[]> validationErrors)
        {
            if (isSuccess && error != Error.None)
            {
                throw new InvalidOperationException();
            }

            if (!isSuccess && error == Error.None)
            {
                throw new InvalidOperationException();
            }

            if (isSuccess && validationErrors != null)
            {
                throw new InvalidOperationException();
            }

            IsSuccess = isSuccess;
            Error = error;
            ValidationErrors = validationErrors;
        }

        public static Result Success() => new(true, Error.None, null);

        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None, null);

        public static Result Failure(Error error, IReadOnlyDictionary<string, string[]> validationErrors = null) => new(false, error, validationErrors);

        public static Result<TValue> Failure<TValue>(Error error, IReadOnlyDictionary<string, string[]> validationErrors = null) => new(default, false, error, validationErrors);

        public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    }
}
