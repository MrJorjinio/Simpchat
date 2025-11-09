using FluentValidation;
using Simpchat.Application.Models.ApiResult;

using System.Diagnostics;

namespace Simpchat.Web.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            try
            {
                await _next(context);
            }
            catch (ValidationException vex)
            {
                //_logger.LogWarning(vex, "Validation failed. TraceId: {TraceId}", traceId);
                //var result = ApiResult<object>.Fail("Validation failed", code: "validation_failed",
                //    status: ResultStatus.ValidationError,
                //    validation: vex.Errors?.GroupBy(e => e.PropertyName)
                //               .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()),
                //    traceId: traceId);

                //context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                //context.Response.ContentType = "application/json";
                //await context.Response.WriteAsJsonAsync(result);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", traceId);
                //var message = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred.";
                //var result = ApiResult<object>.Fail(message, code: "internal_error", status: ResultStatus.Failure);

                //context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //context.Response.ContentType = "application/json";
                //await context.Response.WriteAsJsonAsync(result);
            }
        }
    }
}
