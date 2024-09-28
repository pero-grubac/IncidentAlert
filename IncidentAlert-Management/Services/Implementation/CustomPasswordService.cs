namespace IncidentAlert_Management.Services.Implementation
{
    public class CustomPasswordService : ICustomPasswordService
    {
        public string GenerateRandomPassword(int length = 6, bool requireDigit = true, bool requireLowercase = true, bool requireUppercase = true, bool requireNonAlphanumeric = true, int requiredUniqueChars = 1)
        {
            const string digitChars = "0123456789";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string nonAlphanumericChars = "!@#$%^&*()-_=+[]{}|;:',.<>?";

            Random random = new();
            List<char> passwordChars;
            string password;

            do
            {
                passwordChars = new List<char>();

                // Ensure we meet the required conditions
                if (requireDigit)
                    passwordChars.Add(digitChars[random.Next(digitChars.Length)]);

                if (requireLowercase)
                    passwordChars.Add(lowercaseChars[random.Next(lowercaseChars.Length)]);

                if (requireUppercase)
                    passwordChars.Add(uppercaseChars[random.Next(uppercaseChars.Length)]);

                if (requireNonAlphanumeric)
                    passwordChars.Add(nonAlphanumericChars[random.Next(nonAlphanumericChars.Length)]);

                // Fill the remaining password length
                string allChars = (requireDigit ? digitChars : "") +
                                  (requireLowercase ? lowercaseChars : "") +
                                  (requireUppercase ? uppercaseChars : "") +
                                  (requireNonAlphanumeric ? nonAlphanumericChars : "");

                while (passwordChars.Count < length)
                {
                    passwordChars.Add(allChars[random.Next(allChars.Length)]);
                }

                // Shuffle to ensure randomness
                passwordChars = [.. passwordChars.OrderBy(x => random.Next())];
                password = new string(passwordChars.ToArray());

                // Repeat generation if unique characters requirement is not met
            } while (passwordChars.Distinct().Count() < requiredUniqueChars);

            return password;
        }
    }
}
