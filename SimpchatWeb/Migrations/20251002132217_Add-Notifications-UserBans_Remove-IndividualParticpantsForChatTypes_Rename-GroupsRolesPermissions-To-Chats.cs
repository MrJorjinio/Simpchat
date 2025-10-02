using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpchatWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationsUserBans_RemoveIndividualParticpantsForChatTypes_RenameGroupsRolesPermissionsToChats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelsSubscribers");

            migrationBuilder.DropTable(
                name: "ConversationsMembers");

            migrationBuilder.DropTable(
                name: "GroupRolesPermissions");

            migrationBuilder.DropTable(
                name: "GroupsParticipants");

            migrationBuilder.DropTable(
                name: "GroupsUsersPermissions");

            migrationBuilder.DropTable(
                name: "GroupsUsersRoles");

            migrationBuilder.DropTable(
                name: "GroupPermissions");

            migrationBuilder.DropTable(
                name: "GroupRoles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsSeen",
                table: "Messages");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:chat_privacy_type", "public,private,friends_only")
                .Annotation("Npgsql:Enum:chat_types", "conversation,group,channel")
                .OldAnnotation("Npgsql:Enum:chat_participant_status", "joined,blocked,leaved")
                .OldAnnotation("Npgsql:Enum:chat_privacy_type", "public,private,friends_only")
                .OldAnnotation("Npgsql:Enum:chat_types", "conversation,group,channel")
                .OldAnnotation("Npgsql:Enum:friendships_status", "pending,accepted,blocked")
                .OldAnnotation("Npgsql:Enum:user_status", "active,temporary_blocked,blocked");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "SentAt",
                table: "Messages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<Guid>(
                name: "ReplyId",
                table: "Messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(85)", maxLength: 85, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatsBans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    From = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    To = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatsBans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatsBans_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatsBans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatsParticipants",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LeaveAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatsParticipants", x => new { x.UserId, x.ChatId });
                    table.ForeignKey(
                        name: "FK_ChatsParticipants_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatsParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IsSeen = table.Column<bool>(type: "boolean", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatsUsersPermissions",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatsUsersPermissions", x => new { x.UserId, x.ChatId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_ChatsUsersPermissions_ChatPermissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "ChatPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatsUsersPermissions_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatsUsersPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReplyId",
                table: "Messages",
                column: "ReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatPermissions_Name",
                table: "ChatPermissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatsBans_ChatId",
                table: "ChatsBans",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsBans_UserId",
                table: "ChatsBans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsParticipants_ChatId",
                table: "ChatsParticipants",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsUsersPermissions_ChatId",
                table: "ChatsUsersPermissions",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatsUsersPermissions_PermissionId",
                table: "ChatsUsersPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_MessageId",
                table: "Notifications",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_ReplyId",
                table: "Messages",
                column: "ReplyId",
                principalTable: "Messages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_ReplyId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "ChatsBans");

            migrationBuilder.DropTable(
                name: "ChatsParticipants");

            migrationBuilder.DropTable(
                name: "ChatsUsersPermissions");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "ChatPermissions");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReplyId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ReplyId",
                table: "Messages");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:chat_participant_status", "joined,blocked,leaved")
                .Annotation("Npgsql:Enum:chat_privacy_type", "public,private,friends_only")
                .Annotation("Npgsql:Enum:chat_types", "conversation,group,channel")
                .Annotation("Npgsql:Enum:friendships_status", "pending,accepted,blocked")
                .Annotation("Npgsql:Enum:user_status", "active,temporary_blocked,blocked")
                .OldAnnotation("Npgsql:Enum:chat_privacy_type", "public,private,friends_only")
                .OldAnnotation("Npgsql:Enum:chat_types", "conversation,group,channel");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "SentAt",
                table: "Messages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

            migrationBuilder.AddColumn<bool>(
                name: "IsSeen",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ChannelsSubscribers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LeaveAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ParticipantStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelsSubscribers", x => new { x.UserId, x.ChannelId });
                    table.ForeignKey(
                        name: "FK_ChannelsSubscribers_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelsSubscribers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversationsMembers",
                columns: table => new
                {
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationsMembers", x => new { x.ConversationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ConversationsMembers_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationsMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(85)", maxLength: 85, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupRoles_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupsParticipants",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LeaveAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ParticipantStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupsParticipants", x => new { x.GroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GroupsParticipants_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupsParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupsUsersPermissions",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupsUsersPermissions", x => new { x.UserId, x.GroupId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_GroupsUsersPermissions_GroupPermissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "GroupPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupsUsersPermissions_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupsUsersPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupRolesPermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRolesPermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_GroupRolesPermissions_GroupPermissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "GroupPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupRolesPermissions_GroupRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "GroupRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupsUsersRoles",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupsUsersRoles", x => new { x.GroupId, x.RoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GroupsUsersRoles_GroupRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "GroupRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupsUsersRoles_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupsUsersRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelsSubscribers_ChannelId",
                table: "ChannelsSubscribers",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationsMembers_UserId",
                table: "ConversationsMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermissions_Name",
                table: "GroupPermissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoles_GroupId",
                table: "GroupRoles",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoles_Name",
                table: "GroupRoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupRolesPermissions_PermissionId",
                table: "GroupRolesPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupsParticipants_UserId",
                table: "GroupsParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupsUsersPermissions_GroupId",
                table: "GroupsUsersPermissions",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupsUsersPermissions_PermissionId",
                table: "GroupsUsersPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupsUsersRoles_RoleId",
                table: "GroupsUsersRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupsUsersRoles_UserId",
                table: "GroupsUsersRoles",
                column: "UserId");
        }
    }
}
