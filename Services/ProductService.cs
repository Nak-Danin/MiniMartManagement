using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;
using MiniMartManagement.Services.Exceptions;

namespace MiniMartManagement.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IInventoryRepository inventoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _inventoryRepository = inventoryRepository;
        }

        public List<Product> GetAll() => _productRepository.GetAll();
        public Product? GetById(int productId) => _productRepository.GetById(productId);
        public List<Product> Search(string keyword) => _productRepository.Search(keyword);

        /// <summary>Business rule 3: only Admin can manage products. Also creates the product's initial (zero-stock) Inventory row.</summary>
        public Product AddProduct(
            User actingUser, int categoryId, string productCode, string name,
            decimal purchasePrice, decimal sellingPrice, string unit, int minStock, string? imagePath = null)
        {
            EnsureCanManage(actingUser);

            if (_categoryRepository.GetById(categoryId) == null)
            {
                throw new BusinessRuleException("Selected category does not exist.");
            }

            if (_productRepository.GetByCode(productCode) != null)
            {
                throw new BusinessRuleException($"Product code '{productCode}' is already in use.");
            }

            var product = new Product(
                categoryId, productCode, name, purchasePrice, sellingPrice, unit, minStock,
                isActive: true, createdAt: DateTime.Now, imagePath: imagePath);

            _productRepository.Add(product);
            _inventoryRepository.Add(new Inventory(product.Id, quantity: 0, lastUpdated: DateTime.Now));

            return product;
        }

        public void UpdateProduct(
            User actingUser, Product product, int categoryId, string name,
            decimal purchasePrice, decimal sellingPrice, string unit, int minStock, string? imagePath = null)
        {
            EnsureCanManage(actingUser);

            if (_categoryRepository.GetById(categoryId) == null)
            {
                throw new BusinessRuleException("Selected category does not exist.");
            }

            product.UpdateDetails(categoryId, name, purchasePrice, sellingPrice, unit, minStock);
            product.SetImage(imagePath);
            _productRepository.Update(product);
        }

        public void SetProductActive(User actingUser, int productId, bool isActive)
        {
            EnsureCanManage(actingUser);
            _productRepository.SetActive(productId, isActive);
        }

        private static void EnsureCanManage(User actingUser)
        {
            if (!actingUser.CanManageProductsAndCategories())
            {
                throw new UnauthorizedAccessException("Only an Admin can manage products.");
            }
        }
    }
}
