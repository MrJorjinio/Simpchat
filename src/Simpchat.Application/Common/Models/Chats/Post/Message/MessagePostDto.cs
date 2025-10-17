using Simpchat.Application.Common.Models.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Models.Chats.Post.Message
{
    public class MessagePostDto
    {
        public Guid? ChatId { get; set; }
        public Guid? ReceiverId { get; set; }
        public Guid? ReplyId { get; set; }
        public FileUploadRequest? FileUploadRequest { get; set; }
        public string? Content { get; set; }
    }
}
