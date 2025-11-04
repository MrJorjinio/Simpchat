using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.Users
{
    public class LoginUserDto
    {
        public string Credential { get; set; }
        public string Password { get; set; }
    }
}
