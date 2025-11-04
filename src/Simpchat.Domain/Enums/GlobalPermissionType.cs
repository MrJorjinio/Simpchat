namespace Simpchat.Domain.Enums
{
    public enum GlobalPermissionType
    {
        // User
        SendMessage,
        UpdateOwnMessage,
        JoinGroup,
        JoinChannel,
        ReactToMessage,

        // Moderator
        ManageMessages,
        ManageReactions,
        ManageUsersInGroups,
        ManageUsersInChannels,
        ManageGroupBasics,
        PinMessages,

        // Admin
        FullAccess
    }
}
