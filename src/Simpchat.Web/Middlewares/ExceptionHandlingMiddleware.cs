using FluentValidation;
using Simpchat.Application.Models.ApiResult;

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
            //catch (Exception ex)
            //{

            //}
            await _next(context);
        }
    }
}
