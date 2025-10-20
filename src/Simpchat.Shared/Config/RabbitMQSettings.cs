using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Shared.Config
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
        public string Port { get; set; }
    }
}
