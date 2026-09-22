using MiniMartManagement.Models;

namespace MiniMartManagement.Services
{
    /// <summary>
    /// Tracks who is currently logged in. AuthService sets this on a
    /// successful login and clears it on logout - Forms read
    /// SessionContext.CurrentUser rather than passing the user around
    /// through every constructor.
    /// </summary>
    public static class SessionContext
    {
        public static User? CurrentUser { get; private set; }

        public static bool IsLoggedIn => CurrentUser != null;

        internal static void SetCurrentUser(User user) => CurrentUser = user;

        internal static void Clear() => CurrentUser = null;

        /// <summary>Business rule 1: a user must be logged in before accessing the system.</summary>
        public static User RequireLogin()
        {
            if (CurrentUser == null)
            {
                throw new InvalidOperationException("No user is currently logged in.");
            }

            return CurrentUser;
        }
    }
}
