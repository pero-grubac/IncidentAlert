namespace IncidentAlert_Management.Services
{
    public interface ICustomPasswordService
    {
        string GenerateRandomPassword(int length = 6, bool requireDigit = true, bool requireLowercase = true,
                                     bool requireUppercase = true, bool requireNonAlphanumeric = true,
                                     int requiredUniqueChars = 1);
    }
}
