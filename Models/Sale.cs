namespace MiniMartManagement.Models
{
    /// <summary>
    /// A completed (or in-progress) POS transaction. Owns its SaleItems -
    /// composition, not just association: a SaleItem has no meaning
    /// outside the Sale it belongs to, and the list is only exposed
    /// read-only so items can't be added except through AddItem.
    /// </summary>
    public class Sale
    {
        private readonly List<SaleItem> _items = new();

        public int Id { get; internal set; }
        public int EmployeeId { get; private set; }
        public DateTime SaleDate { get; private set; }
        public decimal Discount { get; private set; }
        public decimal Payment { get; private set; }

        public IReadOnlyList<SaleItem> Items => _items.AsReadOnly();

        public decimal Subtotal => _items.Sum(item => item.Subtotal);
        public decimal Total => Subtotal - Discount;
        public decimal ChangeAmount => Payment - Total;

        public Sale(int employeeId, DateTime saleDate)
        {
            EmployeeId = employeeId;
            SaleDate = saleDate;
        }

        /// <summary>Business rule 7 (quantity > 0) is already enforced by SaleItem's own constructor.</summary>
        public void AddItem(SaleItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            _items.Add(item);
        }

        public void RemoveItem(SaleItem item) => _items.Remove(item);

        public void ApplyDiscount(decimal discount)
        {
            if (discount < 0)
            {
                throw new ArgumentException("Discount cannot be negative.", nameof(discount));
            }

            if (discount > Subtotal)
            {
                throw new ArgumentException("Discount cannot exceed the subtotal.", nameof(discount));
            }

            Discount = discount;
        }

        /// <summary>Business rule 9: payment must cover the total. Rule 10 (change) follows automatically via ChangeAmount.</summary>
        public void SetPayment(decimal payment)
        {
            if (payment < Total)
            {
                throw new InvalidOperationException("Payment must be greater than or equal to the total.");
            }

            Payment = payment;
        }

        /// <summary>
        /// Used only by SaleRepository when reconstructing a Sale that was
        /// already saved to the database. Bypasses ApplyDiscount/SetPayment's
        /// validation (which assumes items are already loaded) because this
        /// data was already validated the first time the sale was completed.
        /// </summary>
        internal void RestoreFinancials(decimal discount, decimal payment)
        {
            Discount = discount;
            Payment = payment;
        }

        /// <summary>Business rule 6: a sale must contain at least one item before it can be completed.</summary>
        public void EnsureCanComplete()
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException("A sale must contain at least one item.");
            }

            if (Payment < Total)
            {
                throw new InvalidOperationException("Payment is insufficient to complete the sale.");
            }
        }
    }
}
