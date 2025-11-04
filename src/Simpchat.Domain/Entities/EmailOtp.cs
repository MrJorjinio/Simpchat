using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Domain.Entities
{
    public class EmailOtp
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Code { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ExpiredAt { get; set; }
    }
}
