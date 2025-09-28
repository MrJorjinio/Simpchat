using SimpchatWeb.Services.Db.Contexts.Default.Enums;

namespace SimpchatWeb.Services.Db.Contexts.Default.Entities
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatedById { get; set; }
        public Chat Chat { get; set; }
        public User UserCreated { get; set; }
        public ICollection<GroupParticipant> Participants { get; set; }
        public ICollection<GroupUserRole> ParticipantsRoles { get; set; }
        public ICollection<GroupUserPermission> ParticipantsPermissions { get; set; }
    }
}
