using System.Security.Cryptography;

namespace MiniMartManagement.Utilities
{
    /// <summary>
    /// PBKDF2-HMAC-SHA256 password hashing. Stored format is
    /// "iterations.saltBase64.hashBase64" - matches the seed data in
    /// Database/Scripts/03_SeedData.sql exactly, so the seeded admin/
    /// employee logins work the first time you run the app.
    /// </summary>
    public static class PasswordHasher
    {
        private const int SaltSizeBytes = 16;
        private const int HashSizeBytes = 32;
        private const int Iterations = 100_000;

        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSizeBytes);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            string[] parts = storedHash.Split('.');
            if (parts.Length != 3)
            {
                return false;
            }

            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] expectedHash = Convert.FromBase64String(parts[2]);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);

            // Constant-time comparison so response timing can't leak how much of the hash matched.
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
