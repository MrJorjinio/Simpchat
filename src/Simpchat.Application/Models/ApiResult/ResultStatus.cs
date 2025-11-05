using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.ApiResults
{
    public enum ResultStatus
    {
        Success = 0,
        Failure = 1,
        NotFound = 2,
        ValidationError = 3,
        Unauthorized = 4,
        Forbidden = 5,
    }
}
