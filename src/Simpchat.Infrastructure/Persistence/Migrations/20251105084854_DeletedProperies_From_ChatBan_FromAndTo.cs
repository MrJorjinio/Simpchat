using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simpchat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DeletedProperies_From_ChatBan_FromAndTo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatsBans_Chats_ChatId",
                table: "ChatsBans");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatsBans_Users_UserId",
                table: "ChatsBans");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersGlobalRoles_GlobalRoles_RoleId",
                table: "UsersGlobalRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersGlobalRoles_Users_UserId",
                table: "UsersGlobalRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersGlobalRoles",
                table: "UsersGlobalRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatsBans",
                table: "ChatsBans");

            migrationBuilder.DropColumn(
                name: "From",
                table: "ChatsBans");

            migrationBuilder.DropColumn(
                name: "To",
                table: "ChatsBans");

            migrationBuilder.RenameTable(
                name: "UsersGlobalRoles",
                newName: "UserGlobalRoles");

            migrationBuilder.RenameTable(
                name: "ChatsBans",
                newName: "ChatBans");

            migrationBuilder.RenameIndex(
                name: "IX_UsersGlobalRoles_RoleId",
                table: "UserGlobalRoles",
                newName: "IX_UserGlobalRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatsBans_UserId",
                table: "ChatBans",
                newName: "IX_ChatBans_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatsBans_ChatId",
                table: "ChatBans",
                newName: "IX_ChatBans_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGlobalRoles",
                table: "UserGlobalRoles",
                columns: new[] { "UserId", "RoleId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatBans",
                table: "ChatBans",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatBans_Chats_ChatId",
                table: "ChatBans",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatBans_Users_UserId",
                table: "ChatBans",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGlobalRoles_GlobalRoles_RoleId",
                table: "UserGlobalRoles",
                column: "RoleId",
                principalTable: "GlobalRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGlobalRoles_Users_UserId",
                table: "UserGlobalRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatBans_Chats_ChatId",
                table: "ChatBans");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatBans_Users_UserId",
                table: "ChatBans");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGlobalRoles_GlobalRoles_RoleId",
                table: "UserGlobalRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGlobalRoles_Users_UserId",
                table: "UserGlobalRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGlobalRoles",
                table: "UserGlobalRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatBans",
                table: "ChatBans");

            migrationBuilder.RenameTable(
                name: "UserGlobalRoles",
                newName: "UsersGlobalRoles");

            migrationBuilder.RenameTable(
                name: "ChatBans",
                newName: "ChatsBans");

            migrationBuilder.RenameIndex(
                name: "IX_UserGlobalRoles_RoleId",
                table: "UsersGlobalRoles",
                newName: "IX_UsersGlobalRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatBans_UserId",
                table: "ChatsBans",
                newName: "IX_ChatsBans_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatBans_ChatId",
                table: "ChatsBans",
                newName: "IX_ChatsBans_ChatId");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "From",
                table: "ChatsBans",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "To",
                table: "ChatsBans",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersGlobalRoles",
                table: "UsersGlobalRoles",
                columns: new[] { "UserId", "RoleId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatsBans",
                table: "ChatsBans",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatsBans_Chats_ChatId",
                table: "ChatsBans",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatsBans_Users_UserId",
                table: "ChatsBans",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
