using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.ApiResult
{
    public sealed class ApiResult<T>
    {
        public bool Success { get; init; }
        public int StatusCode { get; init; }
        public T? Data { get; init; }
        public ApiError? Error { get; init; }
        public IDictionary<string, string[]>? ValidationErrors { get; init; }
    }
}
