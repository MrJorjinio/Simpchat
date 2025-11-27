using FluentValidation;
using Simpchat.Application.Models.ApiResult;
using Simpchat.Shared.Models;
using System.Diagnostics;

namespace Simpchat.Web.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //try
            //{
            //    await _next(context);
            //}
            //catch (ValidationException ex)
            //{
            //    context.Response.ContentType = "application/json";
            //    context.Response.StatusCode = StatusCodes.Status400BadRequest;

            //    var errors = ex.Errors
            //        .GroupBy(e => e.PropertyName)
            //        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            //    var response = new Result<object>();
            //    await context.Response.WriteAsJsonAsync(response);
            //}
            //catch (Exception ex)
            //{
            //    context.Response.ContentType = "application/json";
            //    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            //    var response = new ApiResponse<object>(false, "An unexpected error occurred", null);
            //    await context.Response.WriteAsJsonAsync(response);
            //}
        }
    }
}
