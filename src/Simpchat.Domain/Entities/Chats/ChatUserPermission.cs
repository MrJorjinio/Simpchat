namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class ChatUserPermission
    {
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }
        public Guid PermissionId { get; set; }
        public User User { get; set; }
        public Chat Chat { get; set; }
        public ChatPermission Permission { get; set; }
    }
}
