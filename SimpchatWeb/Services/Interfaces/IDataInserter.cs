using SimpchatWeb.Services.Db.Contexts.Default.Models;

namespace SimpchatWeb.Services.Interfaces
{
    public interface IDataInserter
    {
        void UpsertPermission(GlobalPermissionDto permission);
        void AddPermissionToRole(string roleName, string permissionName);
        void InsertAllSystemPermissions();
        void InsertAllSystemRoles();
    }
}
