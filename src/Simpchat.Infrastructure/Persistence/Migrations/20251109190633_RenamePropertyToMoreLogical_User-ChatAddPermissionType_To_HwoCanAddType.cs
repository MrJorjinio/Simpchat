using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simpchat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamePropertyToMoreLogical_UserChatAddPermissionType_To_HwoCanAddType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChatMemberAddPermissionType",
                table: "Users",
                newName: "HwoCanAddType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HwoCanAddType",
                table: "Users",
                newName: "ChatMemberAddPermissionType");
        }
    }
}
