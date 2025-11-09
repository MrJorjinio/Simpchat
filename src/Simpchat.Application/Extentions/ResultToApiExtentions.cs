using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Models.ApiResult;
using Simpchat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Extentions
{
    public static class ResultToApiResultExtensions
    {
        public static Models.ApiResult.ApiResult<object?> ToApiResult(this Result result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            return new Models.ApiResult.ApiResult<object?>
            {
                Success = result.IsSuccess,
                StatusCode = MapToStatusCode(result.Error, result.ValidationErrors),
                Data = result.IsSuccess ? null : null,
                Error = result.IsSuccess ? null : new ApiError(result.Error.Code, result.Error.Message),
                ValidationErrors = result.ValidationErrors != null
                    ? new Dictionary<string, string[]>(result.ValidationErrors)
                    : null
            };
        }

        public static Models.ApiResult.ApiResult<TValue?> ToApiResult<TValue>(this Result<TValue> result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            return new Models.ApiResult.ApiResult<TValue?>
            {
                Success = result.IsSuccess,
                StatusCode = MapToStatusCode(result.Error, result.ValidationErrors),
                Data = result.IsSuccess ? result.Value : default,
                Error = result.IsSuccess ? null : new ApiError(result.Error.Code, result.Error.Message),
                ValidationErrors = result.ValidationErrors != null
                    ? new Dictionary<string, string[]>(result.ValidationErrors)
                    : null
            };
        }

        private static int MapToStatusCode(Error error, IReadOnlyDictionary<string, string[]>? validationErrors)
        {
            if (validationErrors != null && validationErrors.Any())
                return (int)HttpStatusCode.BadRequest;

            if (error == null) return (int)HttpStatusCode.InternalServerError;

            return error.Code switch
            {
                "Error.NullValue" => (int)HttpStatusCode.NotFound,
                "" or null => (int)HttpStatusCode.OK,
                _ => (int)HttpStatusCode.BadRequest
            };
        }
    }
}
