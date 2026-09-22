using MiniMartManagement.Models;

namespace MiniMartManagement.Interfaces
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        Category? GetById(int categoryId);
        void Add(Category category);
        void Update(Category category);
        void SetActive(int categoryId, bool isActive);
    }
}
