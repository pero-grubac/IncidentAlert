namespace IncidentAlert_Management.Exceptions
{
    /// <summary>The account exists but an administrator has not approved it yet (403).</summary>
    public class AccountPendingException : CustomException
    {
        public AccountPendingException(string message) : base(message) { }
    }
}
