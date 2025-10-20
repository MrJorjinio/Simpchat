using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Models.Pagination.Chat
{
    public class ChatSearchPageModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string searchTerm { get; set; }
    }
}
