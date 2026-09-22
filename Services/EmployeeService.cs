using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;
using MiniMartManagement.Services.Exceptions;
using MiniMartManagement.Utilities;

namespace MiniMartManagement.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IUserRepository userRepository)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
        }

        public List<Employee> GetAll(User actingUser)
        {
            EnsureCanManage(actingUser);
            return _employeeRepository.GetAll();
        }

        public Employee? GetById(User actingUser, int employeeId)
        {
            EnsureCanManage(actingUser);
            return _employeeRepository.GetByEmployeeId(employeeId);
        }

        public List<Employee> Search(User actingUser, string keyword)
        {
            EnsureCanManage(actingUser);
            return _employeeRepository.Search(keyword);
        }

        /// <summary>Business rule 2: only Admin can manage employees.</summary>
        public Employee AddEmployee(
            User actingUser, string username, string password,
            string firstName, string lastName, string? phone, string? email, string? address, DateOnly hireDate)
        {
            EnsureCanManage(actingUser);

            ValidationHelper.EnsureNotEmpty(username, nameof(username));
            ValidationHelper.EnsureNotEmpty(password, nameof(password));
            if (password.Length < 6)
            {
                throw new BusinessRuleException("Password must be at least 6 characters long.");
            }

            if (_userRepository.GetByUsername(username) != null)
            {
                throw new BusinessRuleException($"Username '{username}' is already taken.");
            }

            string passwordHash = PasswordHasher.HashPassword(password);
            var employee = new Employee(
                username, passwordHash, isActive: true, createdAt: DateTime.Now,
                firstName, lastName, phone, email, address, hireDate, EmployeeStatus.Active);

            _employeeRepository.Add(employee);
            return employee;
        }

        public void UpdateEmployee(
            User actingUser, Employee employee,
            string firstName, string lastName, string? phone, string? email, string? address)
        {
            EnsureCanManage(actingUser);
            employee.UpdateProfile(firstName, lastName, phone, email, address);
            _employeeRepository.Update(employee);
        }

        public void SetEmployeeStatus(User actingUser, int employeeId, EmployeeStatus status)
        {
            EnsureCanManage(actingUser);
            _employeeRepository.SetStatus(employeeId, status);
        }

        private static void EnsureCanManage(User actingUser)
        {
            if (!actingUser.CanManageEmployees())
            {
                throw new UnauthorizedAccessException("Only an Admin can manage employees.");
            }
        }
    }
}
