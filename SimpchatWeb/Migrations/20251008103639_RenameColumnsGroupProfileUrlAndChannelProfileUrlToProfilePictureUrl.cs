using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpchatWeb.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnsGroupProfileUrlAndChannelProfileUrlToProfilePictureUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileUrl",
                table: "Groups",
                newName: "ProfilePictureUrl");

            migrationBuilder.RenameColumn(
                name: "ProfileUrl",
                table: "Channels",
                newName: "ProfilePictureUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePictureUrl",
                table: "Groups",
                newName: "ProfileUrl");

            migrationBuilder.RenameColumn(
                name: "ProfilePictureUrl",
                table: "Channels",
                newName: "ProfileUrl");
        }
    }
}
