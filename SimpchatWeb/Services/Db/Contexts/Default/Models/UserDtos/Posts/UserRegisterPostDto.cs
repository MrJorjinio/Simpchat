namespace SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts
{
    public class UserRegisterPostDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }
}
