using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simpchat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIdToAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersGlobalRoles",
                table: "UsersGlobalRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessagesReactions",
                table: "MessagesReactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupsMembers",
                table: "GroupsMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalRolesPermissions",
                table: "GlobalRolesPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatsUsersPermissions",
                table: "ChatsUsersPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChannelsSubscribers",
                table: "ChannelsSubscribers");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UsersGlobalRoles",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reactions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "MessagesReactions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "GroupsMembers",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "GlobalRolesPermissions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ChatsUsersPermissions",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ChannelsSubscribers",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersGlobalRoles",
                table: "UsersGlobalRoles",
                columns: new[] { "UserId", "RoleId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessagesReactions",
                table: "MessagesReactions",
                columns: new[] { "MessageId", "ReactionId", "UserId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupsMembers",
                table: "GroupsMembers",
                columns: new[] { "GroupId", "UserId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalRolesPermissions",
                table: "GlobalRolesPermissions",
                columns: new[] { "RoleId", "PermissionId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatsUsersPermissions",
                table: "ChatsUsersPermissions",
                columns: new[] { "UserId", "ChatId", "PermissionId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelsSubscribers",
                table: "ChannelsSubscribers",
                columns: new[] { "UserId", "ChannelId", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersGlobalRoles",
                table: "UsersGlobalRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessagesReactions",
                table: "MessagesReactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupsMembers",
                table: "GroupsMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalRolesPermissions",
                table: "GlobalRolesPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatsUsersPermissions",
                table: "ChatsUsersPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChannelsSubscribers",
                table: "ChannelsSubscribers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UsersGlobalRoles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MessagesReactions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GroupsMembers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GlobalRolesPermissions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ChatsUsersPermissions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ChannelsSubscribers");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Reactions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersGlobalRoles",
                table: "UsersGlobalRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessagesReactions",
                table: "MessagesReactions",
                columns: new[] { "MessageId", "ReactionId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupsMembers",
                table: "GroupsMembers",
                columns: new[] { "GroupId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalRolesPermissions",
                table: "GlobalRolesPermissions",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatsUsersPermissions",
                table: "ChatsUsersPermissions",
                columns: new[] { "UserId", "ChatId", "PermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelsSubscribers",
                table: "ChannelsSubscribers",
                columns: new[] { "UserId", "ChannelId" });
        }
    }
}
