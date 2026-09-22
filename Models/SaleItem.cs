using MiniMartManagement.Utilities;

namespace MiniMartManagement.Models
{
    /// <summary>
    /// One line item within a Sale. Subtotal is always Quantity * UnitPrice -
    /// computed, never stored separately, so it can't drift out of sync.
    /// </summary>
    public class SaleItem
    {
        public int Id { get; internal set; }
        public int SaleId { get; internal set; }
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public decimal Subtotal => Quantity * UnitPrice;

        public SaleItem(int productId, int quantity, decimal unitPrice)
        {
            ValidationHelper.EnsurePositive(quantity, nameof(quantity));
            ValidationHelper.EnsureNonNegative(unitPrice, nameof(unitPrice));

            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
