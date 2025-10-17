using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SimpchatWeb.Services.Entity
{
    public class ApiResult<T> : IActionResult
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ApiResult(bool success, int statusCode, string message, T? data = default)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(this)
            {
                StatusCode = StatusCode
            };
            await objectResult.ExecuteResultAsync(context);
        }

        public static ApiResult<T> Ok(T data, string message = "Success", int statusCode = 200)
            => new ApiResult<T>(true, statusCode, message, data);

        public static ApiResult<T> Fail(string message = "Failed", int statusCode = 400)
            => new ApiResult<T>(false, statusCode, message);
    }

    public class ApiResult : IActionResult
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; } = null;

        public ApiResult(bool success, int statusCode, string message)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(this)
            {
                StatusCode = StatusCode
            };
            await objectResult.ExecuteResultAsync(context);
        }

        public static ApiResult Ok(string message = "Success", int statusCode = 200)
            => new ApiResult(true, statusCode, message);

        public static ApiResult Fail(string message = "Failed", int statusCode = 400)
            => new ApiResult(false, statusCode, message);
    }
}