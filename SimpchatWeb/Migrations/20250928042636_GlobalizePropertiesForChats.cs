using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpchatWeb.Migrations
{
    /// <inheritdoc />
    public partial class GlobalizePropertiesForChats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Channels");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:chat_participant_status", "joined,blocked,leaved")
                .Annotation("Npgsql:Enum:chat_privacy_type", "public,private,friends_only")
                .Annotation("Npgsql:Enum:chat_types", "conversation,group,channel")
                .Annotation("Npgsql:Enum:friendships_status", "pending,accepted,blocked")
                .Annotation("Npgsql:Enum:user_status", "active,temporary_blocked,blocked")
                .OldAnnotation("Npgsql:Enum:chat_types", "conversation,group,channel")
                .OldAnnotation("Npgsql:Enum:friendships_status", "pending,accepted,blocked");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LeaveAt",
                table: "GroupsParticipants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "ParticipantStatus",
                table: "GroupsParticipants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "GroupRoles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ConversationsMembers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Conversations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Chats",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "PrivacyType",
                table: "Chats",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "JoinedAt",
                table: "ChannelsSubscribers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LeaveAt",
                table: "ChannelsSubscribers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "ParticipantStatus",
                table: "ChannelsSubscribers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoles_GroupId",
                table: "GroupRoles",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupRoles_Groups_GroupId",
                table: "GroupRoles",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupRoles_Groups_GroupId",
                table: "GroupRoles");

            migrationBuilder.DropIndex(
                name: "IX_GroupRoles_GroupId",
                table: "GroupRoles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LeaveAt",
                table: "GroupsParticipants");

            migrationBuilder.DropColumn(
                name: "ParticipantStatus",
                table: "GroupsParticipants");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "GroupRoles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ConversationsMembers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "PrivacyType",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "ChannelsSubscribers");

            migrationBuilder.DropColumn(
                name: "LeaveAt",
                table: "ChannelsSubscribers");

            migrationBuilder.DropColumn(
                name: "ParticipantStatus",
                table: "ChannelsSubscribers");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:chat_types", "conversation,group,channel")
                .Annotation("Npgsql:Enum:friendships_status", "pending,accepted,blocked")
                .OldAnnotation("Npgsql:Enum:chat_participant_status", "joined,blocked,leaved")
                .OldAnnotation("Npgsql:Enum:chat_privacy_type", "public,private,friends_only")
                .OldAnnotation("Npgsql:Enum:chat_types", "conversation,group,channel")
                .OldAnnotation("Npgsql:Enum:friendships_status", "pending,accepted,blocked")
                .OldAnnotation("Npgsql:Enum:user_status", "active,temporary_blocked,blocked");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Channels",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
