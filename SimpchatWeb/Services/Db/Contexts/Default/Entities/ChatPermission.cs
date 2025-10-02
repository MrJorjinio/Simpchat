namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class ChatPermission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<ChatUserPermission> UsersAppliedTo { get; set; }
    }
}
