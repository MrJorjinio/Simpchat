using Simpchat.Domain.Entities;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.Chats.Search
{
    public class SearchChatResponseDto
    {
        public Guid EntityId { get; set; }
        public Guid? ChatId { get; set; }
        public ChatType ChatType { get; set; }
        public string DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
