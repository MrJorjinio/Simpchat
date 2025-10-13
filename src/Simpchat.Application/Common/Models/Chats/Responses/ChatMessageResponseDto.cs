using Simpchat.Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Models.Chats.Response
{
    public class ChatMessageResponseDto
    {
        public Guid ChatId { get; set; }
        public Guid MessageId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReplyId { get; set; }
        public string SenderUsername { get; set; }
        public string? Content { get; set; }
        public string? FileUrl { get; set; }
        public DateTimeOffset SentAt { get; set; }
    }
}
