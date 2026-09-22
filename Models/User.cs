using MiniMartManagement.Utilities;

namespace MiniMartManagement.Models
{
    /// <summary>
    /// Base class for anyone who can log in. Admin and Employee both
    /// inherit from this. Fields are only settable through methods that
    /// validate input - callers can't put a User into a broken state.
    /// </summary>
    public abstract class User
    {
        public int Id { get; internal set; }
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected User(string username, string passwordHash, bool isActive, DateTime createdAt)
        {
            ValidationHelper.EnsureNotEmpty(username, nameof(username));
            ValidationHelper.EnsureNotEmpty(passwordHash, nameof(passwordHash));

            Username = username;
            PasswordHash = passwordHash;
            IsActive = isActive;
            CreatedAt = createdAt;
        }

        /// <summary>Polymorphic - each subclass reports its own role.</summary>
        public abstract UserRole Role { get; }

        /// <summary>Polymorphic - overridden per role so DashboardForm can ask "what should I show?".</summary>
        public virtual string GetDashboardTitle() => "Dashboard";

        /// <summary>Polymorphic authorization hooks - default is "no access".</summary>
        public virtual bool CanManageEmployees() => false;
        public virtual bool CanManageProductsAndCategories() => false;
        public virtual bool CanManageInventory() => false;
        public virtual bool CanViewAllSalesReports() => false;

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void SetPasswordHash(string newPasswordHash)
        {
            ValidationHelper.EnsureNotEmpty(newPasswordHash, nameof(newPasswordHash));
            PasswordHash = newPasswordHash;
        }
    }
}
