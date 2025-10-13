using Simpchat.Application.Common.Models.Users;
using Simpchat.Domain.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Models.Chats.Response
{
    public class UserChatResponseDto
    {
        public Guid Id { get; set; }
        public ChatType ChatType { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfilePicUrl { get; set; }
        public int MembersCount { get; set; }

        public static UserChatResponseDto ConvertFromDomainObject(Chat chat, Guid currentUserId)
        {
            return new UserChatResponseDto
            {
                Id = chat.Id,
                ChatType = chat.Type,
                DisplayName = chat.DisplayName(currentUserId),
                MembersCount = chat.Participants.Count(),
                ProfilePicUrl = chat.DisplayProfilePicUrl(currentUserId)
            };
        }
    }
}
