using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Models.Reactions
{
    public class GetAllMessageReaction
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public int Count { get; set; }
    }
}
