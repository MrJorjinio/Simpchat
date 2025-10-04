using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpchatWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyUserChatMemberAddPermissionPrefixType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChatMemberAddPermission",
                table: "Users",
                newName: "ChatMemberAddPermissionType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChatMemberAddPermissionType",
                table: "Users",
                newName: "ChatMemberAddPermission");
        }
    }
}
