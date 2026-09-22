using MiniMartManagement.Models;

namespace MiniMartManagement.Interfaces
{
    public interface IEmployeeRepository
    {
        Employee? GetByEmployeeId(int employeeId);
        Employee? GetByUserId(int userId);
        List<Employee> GetAll();
        List<Employee> Search(string keyword);

        /// <summary>Creates the Users row and Employees row together in one transaction. Sets employee.Id and employee.EmployeeId on success.</summary>
        void Add(Employee employee);

        /// <summary>Updates profile fields only (name, phone, email, address). Does not touch login/status.</summary>
        void Update(Employee employee);

        /// <summary>Updates Employees.status and the matching Users.is_active together, so login is reliably blocked when deactivated.</summary>
        void SetStatus(int employeeId, EmployeeStatus status);
    }
}
