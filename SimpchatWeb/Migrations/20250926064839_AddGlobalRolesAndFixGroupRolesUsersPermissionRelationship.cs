using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpchatWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalRolesAndFixGroupRolesUsersPermissionRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupsUsersPermissions_GroupRolesPermissions_PermissionId",
                table: "GroupsUsersPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupRolesPermissions",
                table: "GroupRolesPermissions");

            migrationBuilder.DropIndex(
                name: "IX_GroupRolesPermissions_RoleId",
                table: "GroupRolesPermissions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GroupRolesPermissions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "GroupRolesPermissions");

            migrationBuilder.AddColumn<Guid>(
                name: "PermissionId",
                table: "GroupRolesPermissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupRolesPermissions",
                table: "GroupRolesPermissions",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.CreateTable(
                name: "GlobalPermission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(85)", maxLength: 85, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalPermission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlobalRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupPermission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(85)", maxLength: 85, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPermission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlobalRolePermission",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalRolePermission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_GlobalRolePermission_GlobalPermission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "GlobalPermission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GlobalRolePermission_GlobalRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "GlobalRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlobalRoleUser",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalRoleUser", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_GlobalRoleUser_GlobalRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "GlobalRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GlobalRoleUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupRolesPermissions_PermissionId",
                table: "GroupRolesPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalRolePermission_PermissionId",
                table: "GlobalRolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalRoleUser_RoleId",
                table: "GlobalRoleUser",
                column: "RoleId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupRolesPermissions_GroupPermission_PermissionId",
                table: "GroupRolesPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupsUsersPermissions_GroupPermission_PermissionId",
                table: "GroupsUsersPermissions");

            migrationBuilder.DropTable(
                name: "GlobalRolePermission");

            migrationBuilder.DropTable(
                name: "GlobalRoleUser");

            migrationBuilder.DropTable(
                name: "GroupPermission");

            migrationBuilder.DropTable(
                name: "GlobalPermission");

            migrationBuilder.DropTable(
                name: "GlobalRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupRolesPermissions",
                table: "GroupRolesPermissions");

            migrationBuilder.DropIndex(
                name: "IX_GroupRolesPermissions_PermissionId",
                table: "GroupRolesPermissions");

            migrationBuilder.DropColumn(
                name: "PermissionId",
                table: "GroupRolesPermissions");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "GroupRolesPermissions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GroupRolesPermissions",
                type: "character varying(85)",
                maxLength: 85,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupRolesPermissions",
                table: "GroupRolesPermissions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRolesPermissions_RoleId",
                table: "GroupRolesPermissions",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupsUsersPermissions_GroupRolesPermissions_PermissionId",
                table: "GroupsUsersPermissions",
                column: "PermissionId",
                principalTable: "GroupRolesPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
