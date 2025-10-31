using Simpchat.Application.Models.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.Chats
{
    public class PutChatDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public UploadFileRequest? Avatar { get; set; }
    }
}
