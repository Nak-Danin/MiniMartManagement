using MiniMartManagement.Utilities;

namespace MiniMartManagement.Models
{
    /// <summary>
    /// An Employee is a User plus the extra profile data needed for
    /// staff records (name, contact info, hire date, employment status).
    /// </summary>
    public sealed class Employee : User
    {
        /// <summary>
        /// Primary key of the Employees table row - distinct from
        /// Id (which is Users.user_id, inherited from User).
        /// </summary>
        public int EmployeeId { get; internal set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string? Phone { get; private set; }
        public string? Email { get; private set; }
        public string? Address { get; private set; }
        public DateOnly HireDate { get; private set; }
        public EmployeeStatus Status { get; private set; }

        public string FullName => $"{FirstName} {LastName}";

        public Employee(
            string username, string passwordHash, bool isActive, DateTime createdAt,
            string firstName, string lastName, string? phone, string? email, string? address,
            DateOnly hireDate, EmployeeStatus status)
            : base(username, passwordHash, isActive, createdAt)
        {
            ValidationHelper.EnsureNotEmpty(firstName, nameof(firstName));
            ValidationHelper.EnsureNotEmpty(lastName, nameof(lastName));
            if (!string.IsNullOrWhiteSpace(email) && !ValidationHelper.IsValidEmail(email))
            {
                throw new ArgumentException("Email address is not valid.", nameof(email));
            }

            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Email = email;
            Address = address;
            HireDate = hireDate;
            Status = status;
        }

        public override UserRole Role => UserRole.Employee;

        public override string GetDashboardTitle() => "Employee POS Dashboard";

        /// <summary>
        /// Business rule 13: inactive employees cannot log in. Checks both
        /// the Users.is_active flag (inherited) and the Employees.status field.
        /// </summary>
        public bool IsAvailableForLogin() => IsActive && Status == EmployeeStatus.Active;

        public void UpdateProfile(string firstName, string lastName, string? phone, string? email, string? address)
        {
            ValidationHelper.EnsureNotEmpty(firstName, nameof(firstName));
            ValidationHelper.EnsureNotEmpty(lastName, nameof(lastName));
            if (!string.IsNullOrWhiteSpace(email) && !ValidationHelper.IsValidEmail(email))
            {
                throw new ArgumentException("Email address is not valid.", nameof(email));
            }

            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Email = email;
            Address = address;
        }

        public void SetStatus(EmployeeStatus status) => Status = status;
    }
}
