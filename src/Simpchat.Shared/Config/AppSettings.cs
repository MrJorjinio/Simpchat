namespace Simpchat.Shared.Config
{
    public class AppSettings
    {
        public JwtSettings JwtSettings { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public MinioSettings MinioSettings { get; set; }
    }
}
