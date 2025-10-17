using Simpchat.Application.Common.Models.ApiResults.Enums;

namespace Simpchat.Application.Common.Models.ApiResults
{
    public class ApiResult<T>
    {
        public ResultStatus Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public bool IsSuccess => Status == ResultStatus.Success;

        public static ApiResult<T> SuccessResult(T data, string message = "Operation successful")
        {
            return new ApiResult<T>
            {
                Status = ResultStatus.Success,
                Message = message,
                Data = data
            };
        }

        public static ApiResult<T> FailureResult(string message, ResultStatus resultStatus = ResultStatus.Failure)
        {
            return new ApiResult<T>
            {
                Status = resultStatus,
                Message = message
            };
        }
    }

    public class ApiResult
    {
        public ResultStatus Status { get; set; }
        public string Message { get; set; }
        public bool IsSuccess => Status == ResultStatus.Success;

        public static ApiResult SuccessResult(string message = "Operation successful")
        {
            return new ApiResult
            {
                Status = ResultStatus.Success,
                Message = message
            };
        }

        public static ApiResult FailureResult(string message, ResultStatus resultStatus = ResultStatus.Failure)
        {
            return new ApiResult
            {
                Message = message,
                Status = resultStatus
            };
        }
    }
}