namespace SimpchatWeb.Services.Settings
{
    public class MinioSettings
    {
        public string Endpoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public bool UseSsl { get; set; }
    }
}
