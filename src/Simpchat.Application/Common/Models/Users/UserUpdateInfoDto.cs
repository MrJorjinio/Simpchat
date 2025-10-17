using SimpchatWeb.Services.Db.Contexts.Default.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Common.Models.Users
{
    public class UserUpdateInfoDto
    {
        public string Username { get; set; }
        public string Description { get; set; }
        public ChatMemberAddPermissionType AddChatMinLvl { get; set; }
    }
}
