using MiniMartManagement.Models;

namespace MiniMartManagement.Interfaces
{
    /// <summary>
    /// Read/write access to login accounts (Users table). Returns the
    /// correct concrete type (Admin or Employee) based on role - callers
    /// work with the abstract User type and don't need to know which.
    /// </summary>
    public interface IUserRepository
    {
        User? GetByUsername(string username);
        User? GetById(int userId);
        void UpdatePasswordHash(int userId, string newPasswordHash);
        void SetActive(int userId, bool isActive);
    }
}
