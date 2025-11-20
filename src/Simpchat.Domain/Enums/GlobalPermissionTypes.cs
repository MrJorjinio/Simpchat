namespace Simpchat.Domain.Enums
{
    public enum GlobalPermissionTypes
    {
        // Moderator
        DeleteAnyMessage,
        EditGroupInfo,
        PinOrUnpinMessages,
        ManageUserReactions,
        MuteOrBanUsers,
        InviteUsers,

        // Admin
        ManageRoles,
        ManagePermissions,
        ManageSystemSettings,
        DeleteGroups,
        DeleteChannels,
        AccessAllData,
        ManageInfrastructure
    }

}
