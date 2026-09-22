namespace MiniMartManagement.Models
{
    /// <summary>
    /// Tracks stock for one product. Quantity can only change through
    /// Increase/Decrease, which enforce that stock never goes negative
    /// (business rule 5 / 11) - callers can't set Quantity directly.
    /// </summary>
    public class Inventory
    {
        public int Id { get; internal set; }
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public DateTime LastUpdated { get; private set; }

        public Inventory(int productId, int quantity, DateTime lastUpdated)
        {
            if (quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
            }

            ProductId = productId;
            Quantity = quantity;
            LastUpdated = lastUpdated;
        }

        public bool IsOutOfStock => Quantity <= 0;

        /// <summary>Business rule 12: low-stock warning once quantity drops to/below the product's MinStock.</summary>
        public bool IsLowStock(int minStockLevel) => Quantity > 0 && Quantity <= minStockLevel;

        public bool HasSufficientStock(int requestedQuantity) => Quantity >= requestedQuantity;

        public void Increase(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            }

            Quantity += amount;
            LastUpdated = DateTime.Now;
        }

        /// <summary>Business rule 11: NewStock = CurrentStock - QuantitySold. Never allows a negative result.</summary>
        public void Decrease(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            }

            if (amount > Quantity)
            {
                throw new InvalidOperationException(
                    $"Cannot decrease stock by {amount}; only {Quantity} unit(s) available.");
            }

            Quantity -= amount;
            LastUpdated = DateTime.Now;
        }
    }
}
