using MiniMartManagement.Models;

namespace MiniMartManagement.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetAll();
        Product? GetById(int productId);
        Product? GetByCode(string productCode);
        List<Product> Search(string keyword);
        void Add(Product product);
        void Update(Product product);
        void SetActive(int productId, bool isActive);
    }
}
