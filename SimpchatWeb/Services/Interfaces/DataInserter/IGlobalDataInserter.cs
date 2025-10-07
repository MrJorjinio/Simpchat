using SimpchatWeb.Services.Db.Contexts.Default.Models.GlobalPermissions;
using System.Threading.Tasks;

namespace SimpchatWeb.Services.Interfaces.DataInserter
{
    public interface IGlobalDataInserter
    {
        Task UpsertPermissionAsync(GlobalPermissionDto permission);
        Task AddPermissionToRoleAsync(string roleName, string permissionName);
        Task InsertSysPermissionsAsync();
        Task InsertSysRolesAsync();
    }
}
