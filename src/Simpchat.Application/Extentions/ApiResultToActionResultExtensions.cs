using Microsoft.AspNetCore.Mvc;
using Simpchat.Application.Models.ApiResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Extentions
{
    public static class ApiResultToActionResultExtensions
    {
        public static IActionResult ToActionResult<T>(this ApiResult<T> apiResult)
        {
            if (apiResult == null) return new StatusCodeResult(500);

            var objectResult = new ObjectResult(apiResult)
            {
                StatusCode = apiResult.StatusCode == 0 ? 200 : apiResult.StatusCode
            };

            return objectResult;
        }
    }
}
