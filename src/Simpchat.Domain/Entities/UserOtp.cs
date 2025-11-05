using Simpchat.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Domain.Entities
{
    public class UserOtp : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Code { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ExpiredAt { get; set; }
        public User User { get; set; }
    }
}
