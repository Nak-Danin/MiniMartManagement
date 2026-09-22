using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;
using MiniMartManagement.Services.Exceptions;

namespace MiniMartManagement.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // Reading categories is left open to any caller (no actingUser check) - Product
        // management and the POS product list both need category names, and read access
        // on its own doesn't touch anything the spec is trying to protect. Only the
        // mutating operations below enforce Admin-only (business rule 3).
        public List<Category> GetAll() => _categoryRepository.GetAll();

        public Category? GetById(int categoryId) => _categoryRepository.GetById(categoryId);

        public Category AddCategory(User actingUser, string name, string? description)
        {
            EnsureCanManage(actingUser);

            bool duplicate = _categoryRepository.GetAll()
                .Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
            if (duplicate)
            {
                throw new BusinessRuleException($"A category named '{name}' already exists.");
            }

            var category = new Category(name, description, isActive: true);
            _categoryRepository.Add(category);
            return category;
        }

        public void UpdateCategory(User actingUser, Category category, string name, string? description)
        {
            EnsureCanManage(actingUser);
            category.UpdateDetails(name, description);
            _categoryRepository.Update(category);
        }

        public void SetCategoryActive(User actingUser, int categoryId, bool isActive)
        {
            EnsureCanManage(actingUser);
            _categoryRepository.SetActive(categoryId, isActive);
        }

        private static void EnsureCanManage(User actingUser)
        {
            if (!actingUser.CanManageProductsAndCategories())
            {
                throw new UnauthorizedAccessException("Only an Admin can manage categories.");
            }
        }
    }
}
