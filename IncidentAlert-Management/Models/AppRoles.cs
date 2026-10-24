namespace IncidentAlert_Management.Models
{
    /// <summary>Role names as they appear in the JWT, for [Authorize(Roles = ...)].</summary>
    public static class AppRoles
    {
        public const string Admin = nameof(RoleEnum.ADMIN);
        public const string Moderator = nameof(RoleEnum.MODERATOR);

        /// <summary>Everyone who may work the moderation queue. Administrators can moderate too.</summary>
        public const string Staff = Moderator + "," + Admin;
    }
}
