using MiniMartManagement.Models;

namespace MiniMartManagement.Interfaces
{
    public interface IInventoryRepository
    {
        Inventory? GetByProductId(int productId);
        List<Inventory> GetAll();

        /// <summary>Products where 0 &lt; quantity &lt;= min_stock.</summary>
        List<Inventory> GetLowStock();

        /// <summary>Products where quantity &lt;= 0.</summary>
        List<Inventory> GetOutOfStock();

        /// <summary>Creates the initial inventory row for a newly-added product.</summary>
        void Add(Inventory inventory);

        /// <summary>Manual admin stock correction (not sale-driven).</summary>
        void AdjustQuantity(int productId, int newQuantity);
    }
}
