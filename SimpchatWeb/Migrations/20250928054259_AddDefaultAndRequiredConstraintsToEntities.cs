using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpchatWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultAndRequiredConstraintsToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "RegisteredAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastSeen",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Users",
                type: "character varying(85)",
                maxLength: 85,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(85)",
                oldMaxLength: 85);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "FormedAt",
                table: "Friendships",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "PrivacyType",
                table: "Chats",
                type: "text",
                nullable: false,
                defaultValue: "Public",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Chats",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Active");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "RegisteredAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastSeen",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Users",
                type: "character varying(85)",
                maxLength: 85,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(85)",
                oldMaxLength: 85,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "FormedAt",
                table: "Friendships",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            migrationBuilder.AlterColumn<string>(
                name: "PrivacyType",
                table: "Chats",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Public");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Chats",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
        }
    }
}
