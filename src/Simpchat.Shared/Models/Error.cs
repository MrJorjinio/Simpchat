using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Shared.Models
{
    public class Error
    {
        public string Code { get; }
        public string Message { get; }


        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
