using System.Text.RegularExpressions;

namespace MiniMartManagement.Utilities
{
    /// <summary>
    /// Small set of reusable validation checks so model classes don't
    /// each re-implement the same null/empty/range checks.
    /// </summary>
    public static class ValidationHelper
    {
        private static readonly Regex EmailPattern =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public static void EnsureNotEmpty(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{fieldName} is required.", fieldName);
            }
        }

        public static void EnsureNonNegative(decimal value, string fieldName)
        {
            if (value < 0)
            {
                throw new ArgumentException($"{fieldName} cannot be negative.", fieldName);
            }
        }

        public static void EnsurePositive(int value, string fieldName)
        {
            if (value <= 0)
            {
                throw new ArgumentException($"{fieldName} must be greater than zero.", fieldName);
            }
        }

        public static bool IsValidEmail(string email) => EmailPattern.IsMatch(email);
    }
}
