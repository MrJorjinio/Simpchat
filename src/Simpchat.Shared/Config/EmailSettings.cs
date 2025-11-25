using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Shared.Config
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string DefaultFromEmail { get; set; }
        public string DefaultFromName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
