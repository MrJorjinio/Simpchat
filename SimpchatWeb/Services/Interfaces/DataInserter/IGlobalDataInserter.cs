using SimpchatWeb.Services.Db.Contexts.Default.Models.GlobalPermissions;

namespace SimpchatWeb.Services.Interfaces.DataInserter
{
    public interface IGlobalDataInserter
    {
        void UpsertPermission(
            GlobalPermissionDto permission
            );
        void AddPermissionToRole(
            string roleName,
            string permissionName
            );
        void InsertSysPermissions();
        void InsertSysRoles();
    }
}
