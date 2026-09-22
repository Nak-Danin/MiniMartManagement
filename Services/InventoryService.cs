using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;
using MiniMartManagement.Services.Exceptions;

namespace MiniMartManagement.Services
{
    public class InventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public List<Inventory> GetAll() => _inventoryRepository.GetAll();
        public Inventory? GetByProductId(int productId) => _inventoryRepository.GetByProductId(productId);

        /// <summary>Business rule 12: products at or below their MinStock level.</summary>
        public List<Inventory> GetLowStock() => _inventoryRepository.GetLowStock();

        public List<Inventory> GetOutOfStock() => _inventoryRepository.GetOutOfStock();

        /// <summary>Manual admin stock correction (restocking, damage write-off, etc.) - not sale-driven, so it's a plain set rather than Increase/Decrease.</summary>
        public void AdjustStock(User actingUser, int productId, int newQuantity)
        {
            if (!actingUser.CanManageInventory())
            {
                throw new UnauthorizedAccessException("Only an Admin can manage inventory.");
            }

            if (newQuantity < 0)
            {
                throw new BusinessRuleException("Quantity cannot be negative.");
            }

            _inventoryRepository.AdjustQuantity(productId, newQuantity);
        }
    }
}
