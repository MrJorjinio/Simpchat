namespace SimpchatWeb.Services.Db.Contexts.Default.Models.GlobalRolesDtos
{
    public class GlobalRoleUserDto
    {
        public string Username { get; set; }
        public ICollection<GlobalRoleDto> Roles { get; set; }
    }
}
