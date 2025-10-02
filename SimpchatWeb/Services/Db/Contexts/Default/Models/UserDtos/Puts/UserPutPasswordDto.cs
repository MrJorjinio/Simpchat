namespace SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Puts
{
    public class UserPutPasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
