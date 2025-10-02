using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpchatWeb.Migrations
{
    /// <inheritdoc />
    public partial class RenameGlobalRolesUsersToUsersGlobalRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlobalRolesUsers_GlobalRoles_RoleId",
                table: "GlobalRolesUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalRolesUsers_Users_UserId",
                table: "GlobalRolesUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalRolesUsers",
                table: "GlobalRolesUsers");

            migrationBuilder.RenameTable(
                name: "GlobalRolesUsers",
                newName: "UsersGlobalRoles");

            migrationBuilder.RenameIndex(
                name: "IX_GlobalRolesUsers_RoleId",
                table: "UsersGlobalRoles",
                newName: "IX_UsersGlobalRoles_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersGlobalRoles",
                table: "UsersGlobalRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UsersGlobalRoles_GlobalRoles_RoleId",
                table: "UsersGlobalRoles",
                column: "RoleId",
                principalTable: "GlobalRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersGlobalRoles_Users_UserId",
                table: "UsersGlobalRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersGlobalRoles_GlobalRoles_RoleId",
                table: "UsersGlobalRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersGlobalRoles_Users_UserId",
                table: "UsersGlobalRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersGlobalRoles",
                table: "UsersGlobalRoles");

            migrationBuilder.RenameTable(
                name: "UsersGlobalRoles",
                newName: "GlobalRolesUsers");

            migrationBuilder.RenameIndex(
                name: "IX_UsersGlobalRoles_RoleId",
                table: "GlobalRolesUsers",
                newName: "IX_GlobalRolesUsers_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalRolesUsers",
                table: "GlobalRolesUsers",
                columns: new[] { "UserId", "RoleId" });

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
        }
    }
}
