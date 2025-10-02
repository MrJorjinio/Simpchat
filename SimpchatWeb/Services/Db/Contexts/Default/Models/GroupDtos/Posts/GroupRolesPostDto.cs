namespace SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos.Posts
{
    public class GroupRolesPostDto
    {
        public Guid Id { get; set; }
        public ICollection<GroupRolePostDto> Roles { get; set; }
    }
}
