namespace SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos
{
    public class UserUpdatePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
