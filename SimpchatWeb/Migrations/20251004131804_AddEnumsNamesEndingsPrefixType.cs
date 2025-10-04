using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpchatWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddEnumsNamesEndingsPrefixType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:chat_member_add_permision_type", "everyone,with_conversations,nobody")
                .Annotation("Npgsql:Enum:chat_privacy_type", "public,private")
                .Annotation("Npgsql:Enum:chat_type", "conversation,group,channel")
                .OldAnnotation("Npgsql:Enum:chat_member_add_permision", "everyone,with_conversations,nobody")
                .OldAnnotation("Npgsql:Enum:chat_privacy_type", "public,private")
                .OldAnnotation("Npgsql:Enum:chat_types", "conversation,group,channel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:chat_member_add_permision", "everyone,with_conversations,nobody")
                .Annotation("Npgsql:Enum:chat_privacy_type", "public,private")
                .Annotation("Npgsql:Enum:chat_types", "conversation,group,channel")
                .OldAnnotation("Npgsql:Enum:chat_member_add_permision_type", "everyone,with_conversations,nobody")
                .OldAnnotation("Npgsql:Enum:chat_privacy_type", "public,private")
                .OldAnnotation("Npgsql:Enum:chat_type", "conversation,group,channel");
        }
    }
}
