using Microsoft.Extensions.Configuration;

namespace Simpchat.Shared.Config
{
    public class AppSettings
    {
        public JwtSettings JwtSettings { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public MinioSettings MinioSettings { get; set; }
        public RabbitMQSettings RabbitMQSettings { get; set; }
        public EmailSettings EmailSettings { get; set; }
    }
}
