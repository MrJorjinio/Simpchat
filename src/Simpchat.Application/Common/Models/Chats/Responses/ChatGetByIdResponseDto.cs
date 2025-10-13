using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpchat.Application.Common.Models.Users;
using Simpchat.Domain.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;

namespace Simpchat.Application.Common.Models.Chats.Response
{
    public class ChatGetByIdResponseDto
    {
        public Guid Id { get; set; }
        public ChatType ChatType { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfilePicUrl { get; set; }
        public ICollection<UserResponseDto> Participants { get; set; }

        public static ChatGetByIdResponseDto ConvertFromDomainObject(Chat chat, Guid currentUserId)
        {
            return new ChatGetByIdResponseDto
            {
                ChatType = chat.Type,
                DisplayName = chat.DisplayName(currentUserId),
                Participants = chat.Participants
                    .Select(u => UserResponseDto.ConvertFromDomainObject(u.User))
                    .ToList(),
                ProfilePicUrl = chat.DisplayProfilePicUrl(currentUserId)
            };
        }
    }
}
