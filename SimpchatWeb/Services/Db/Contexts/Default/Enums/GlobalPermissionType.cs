namespace SimpchatWeb.Services.Db.Contexts.Default.Enums
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
