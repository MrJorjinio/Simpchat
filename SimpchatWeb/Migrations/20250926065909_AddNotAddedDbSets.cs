using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpchatWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddNotAddedDbSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalRolePermission_GlobalPermission_PermissionId",
                table: "GlobalRolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalRolePermission_GlobalRole_RoleId",
                table: "GlobalRolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalRoleUser_GlobalRole_RoleId",
                table: "GlobalRoleUser");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalRoleUser_Users_UserId",
                table: "GlobalRoleUser");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupRolesPermissions_GroupPermission_PermissionId",
                table: "GroupRolesPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupsUsersPermissions_GroupPermission_PermissionId",
                table: "GroupsUsersPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupPermission",
                table: "GroupPermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalRoleUser",
                table: "GlobalRoleUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalRolePermission",
                table: "GlobalRolePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalRole",
                table: "GlobalRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalPermission",
                table: "GlobalPermission");

            migrationBuilder.RenameTable(
                name: "GroupPermission",
                newName: "GroupPermissions");

            migrationBuilder.RenameTable(
                name: "GlobalRoleUser",
                newName: "GlobalRolesUsers");

            migrationBuilder.RenameTable(
                name: "GlobalRolePermission",
                newName: "GlobalRolesPermissions");

            migrationBuilder.RenameTable(
                name: "GlobalRole",
                newName: "GlobalRoles");

            migrationBuilder.RenameTable(
                name: "GlobalPermission",
                newName: "GlobalPermissions");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalRoleUser_RoleId",
                table: "GlobalRolesUsers",
                newName: "IX_GlobalRolesUsers_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalRolePermission_PermissionId",
                table: "GlobalRolesPermissions",
                newName: "IX_GlobalRolesPermissions_PermissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupPermissions",
                table: "GroupPermissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalRolesUsers",
                table: "GlobalRolesUsers",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalRolesPermissions",
                table: "GlobalRolesPermissions",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalRoles",
                table: "GlobalRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalPermissions",
                table: "GlobalPermissions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalRolesPermissions_GlobalPermissions_PermissionId",
                table: "GlobalRolesPermissions",
                column: "PermissionId",
                principalTable: "GlobalPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalRolesPermissions_GlobalRoles_RoleId",
                table: "GlobalRolesPermissions",
                column: "RoleId",
                principalTable: "GlobalRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalRolesUsers_GlobalRoles_RoleId",
                table: "GlobalRolesUsers",
                column: "RoleId",
                principalTable: "GlobalRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalRolesUsers_Users_UserId",
                table: "GlobalRolesUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupRolesPermissions_GroupPermissions_PermissionId",
                table: "GroupRolesPermissions",
                column: "PermissionId",
                principalTable: "GroupPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupsUsersPermissions_GroupPermissions_PermissionId",
                table: "GroupsUsersPermissions",
                column: "PermissionId",
                principalTable: "GroupPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalRolesPermissions_GlobalPermissions_PermissionId",
                table: "GlobalRolesPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalRolesPermissions_GlobalRoles_RoleId",
                table: "GlobalRolesPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalRolesUsers_GlobalRoles_RoleId",
                table: "GlobalRolesUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalRolesUsers_Users_UserId",
                table: "GlobalRolesUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupRolesPermissions_GroupPermissions_PermissionId",
                table: "GroupRolesPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupsUsersPermissions_GroupPermissions_PermissionId",
                table: "GroupsUsersPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupPermissions",
                table: "GroupPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalRolesUsers",
                table: "GlobalRolesUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalRolesPermissions",
                table: "GlobalRolesPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalRoles",
                table: "GlobalRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalPermissions",
                table: "GlobalPermissions");

            migrationBuilder.RenameTable(
                name: "GroupPermissions",
                newName: "GroupPermission");

            migrationBuilder.RenameTable(
                name: "GlobalRolesUsers",
                newName: "GlobalRoleUser");

            migrationBuilder.RenameTable(
                name: "GlobalRolesPermissions",
                newName: "GlobalRolePermission");

            migrationBuilder.RenameTable(
                name: "GlobalRoles",
                newName: "GlobalRole");

            migrationBuilder.RenameTable(
                name: "GlobalPermissions",
                newName: "GlobalPermission");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalRolesUsers_RoleId",
                table: "GlobalRoleUser",
                newName: "IX_GlobalRoleUser_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalRolesPermissions_PermissionId",
                table: "GlobalRolePermission",
                newName: "IX_GlobalRolePermission_PermissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupPermission",
                table: "GroupPermission",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalRoleUser",
                table: "GlobalRoleUser",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalRolePermission",
                table: "GlobalRolePermission",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalRole",
                table: "GlobalRole",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalPermission",
                table: "GlobalPermission",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalRolePermission_GlobalPermission_PermissionId",
                table: "GlobalRolePermission",
                column: "PermissionId",
                principalTable: "GlobalPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalRolePermission_GlobalRole_RoleId",
                table: "GlobalRolePermission",
                column: "RoleId",
                principalTable: "GlobalRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalRoleUser_GlobalRole_RoleId",
                table: "GlobalRoleUser",
                column: "RoleId",
                principalTable: "GlobalRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalRoleUser_Users_UserId",
                table: "GlobalRoleUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupRolesPermissions_GroupPermission_PermissionId",
                table: "GroupRolesPermissions",
                column: "PermissionId",
                principalTable: "GroupPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupsUsersPermissions_GroupPermission_PermissionId",
                table: "GroupsUsersPermissions",
                column: "PermissionId",
                principalTable: "GroupPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
