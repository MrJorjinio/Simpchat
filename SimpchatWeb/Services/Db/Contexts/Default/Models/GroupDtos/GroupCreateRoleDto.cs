namespace SimpchatWeb.Services.Db.Contexts.Default.Models.GroupDtos
{
    public class GroupCreateRoleDto
    {
        public Guid Id { get; set; }
        public ICollection<GroupRoleDto> Roles { get; set; }
    }
}
