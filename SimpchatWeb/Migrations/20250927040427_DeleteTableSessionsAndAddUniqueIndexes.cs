using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpchatWeb.Migrations
{
    /// <inheritdoc />
    public partial class DeleteTableSessionsAndAddUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "GlobalPermissions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Friendships",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Chats",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoles_Name",
                table: "GroupRoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermissions_Name",
                table: "GroupPermissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalRoles_Name",
                table: "GlobalRoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalPermissions_Name",
                table: "GlobalPermissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Name",
                table: "Channels",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Groups_Name",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_GroupRoles_Name",
                table: "GroupRoles");

            migrationBuilder.DropIndex(
                name: "IX_GroupPermissions_Name",
                table: "GroupPermissions");

            migrationBuilder.DropIndex(
                name: "IX_GlobalRoles_Name",
                table: "GlobalRoles");

            migrationBuilder.DropIndex(
                name: "IX_GlobalPermissions_Name",
                table: "GlobalPermissions");

            migrationBuilder.DropIndex(
                name: "IX_Channels_Name",
                table: "Channels");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "GlobalPermissions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Friendships",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Chats",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConnectedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ConnectionId = table.Column<string>(type: "text", nullable: false),
                    Device = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DisconnectedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");
        }
    }
}
