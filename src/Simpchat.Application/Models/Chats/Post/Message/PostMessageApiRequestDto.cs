using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.Chats.Post.Message
{
    public class PostMessageApiRequestDto
    {
        public Guid? ChatId { get; set; }
        public Guid? ReceiverId { get; set; }
        public Guid? ReplyId { get; set; }
        public string? Content { get; set; }
    }
}
