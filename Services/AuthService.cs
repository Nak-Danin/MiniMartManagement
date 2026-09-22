using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;
using MiniMartManagement.Services.Exceptions;
using MiniMartManagement.Utilities;

namespace MiniMartManagement.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Verifies credentials and account status, then sets SessionContext.CurrentUser.
        /// The same error message is used whether the username doesn't exist or the
        /// password is wrong, so a failed attempt can't be used to guess valid usernames.
        /// </summary>
        public User Login(string username, string password)
        {
            ValidationHelper.EnsureNotEmpty(username, nameof(username));
            ValidationHelper.EnsureNotEmpty(password, nameof(password));

            User? user = _userRepository.GetByUsername(username);
            if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                throw new AuthenticationException("Invalid username or password.");
            }

            // Business rule 13: inactive accounts cannot log in.
            if (!user.IsActive)
            {
                throw new AuthenticationException("This account has been deactivated. Contact an administrator.");
            }

            if (user is Employee employee && employee.Status == EmployeeStatus.Inactive)
            {
                throw new AuthenticationException("This employee account is inactive. Contact an administrator.");
            }

            SessionContext.SetCurrentUser(user);
            return user;
        }

        public void Logout() => SessionContext.Clear();

        public void ChangePassword(User user, string currentPassword, string newPassword)
        {
            if (!PasswordHasher.VerifyPassword(currentPassword, user.PasswordHash))
            {
                throw new AuthenticationException("Current password is incorrect.");
            }

            ValidationHelper.EnsureNotEmpty(newPassword, nameof(newPassword));
            if (newPassword.Length < 6)
            {
                throw new BusinessRuleException("New password must be at least 6 characters long.");
            }

            string newHash = PasswordHasher.HashPassword(newPassword);
            _userRepository.UpdatePasswordHash(user.Id, newHash);
            user.SetPasswordHash(newHash);
        }
    }
}
