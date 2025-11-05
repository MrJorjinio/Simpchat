using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.ApiResult
{
    public class ApiError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public IDictionary<string, string[]> Validation { get; set; }
        public string Details { get; set; }
    }
}
