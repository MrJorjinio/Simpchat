namespace SimpchatWeb.Services.Db.Contexts.Default.Models.UserDtos.Posts
{
    public class UserGlobalRolesPostDto
    {
        public string Username { get; set; }
        public ICollection<string> RoleNames { get; set; }
    }
}
