using Simpchat.Application.Common.Models.Chats.Response;
using Simpchat.Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Models.Chats.Messages
{
    public class ChatSendMessageDto
    {
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReplyId { get; set; }
        public string? Content { get; set; }
        public Stream? FileStream { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }

        public static Message ToDomainObject(ChatSendMessageDto model)
        {
            return new Message
            {
                ChatId = model.ChatId,
                SenderId = model.SenderId,
                Content = model.Content,
                ReplyId = model.ReplyId
            };
        }
    }
}
