using MiniMartManagement.Utilities;

namespace MiniMartManagement.Models
{
    public class Product
    {
        public int Id { get; internal set; }
        public int CategoryId { get; private set; }
        public string ProductCode { get; private set; }
        public string Name { get; private set; }
        public decimal PurchasePrice { get; private set; }
        public decimal SellingPrice { get; private set; }
        public string Unit { get; private set; }
        public int MinStock { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Path to the product's photo, relative to the application's base directory
        /// (e.g. "ProductImages/3f1c...jpg"). Null/empty means "no photo yet" - the UI
        /// falls back to a drawn placeholder rather than treating this as an error.
        /// </summary>
        public string? ImagePath { get; private set; }

        public Product(
            int categoryId, string productCode, string name,
            decimal purchasePrice, decimal sellingPrice, string unit,
            int minStock, bool isActive, DateTime createdAt, string? imagePath = null)
        {
            ValidationHelper.EnsureNotEmpty(productCode, nameof(productCode));
            ValidationHelper.EnsureNotEmpty(name, nameof(name));
            ValidationHelper.EnsureNotEmpty(unit, nameof(unit));
            ValidationHelper.EnsureNonNegative(purchasePrice, nameof(purchasePrice));
            ValidationHelper.EnsureNonNegative(sellingPrice, nameof(sellingPrice));
            if (minStock < 0)
            {
                throw new ArgumentException("Minimum stock cannot be negative.", nameof(minStock));
            }

            CategoryId = categoryId;
            ProductCode = productCode;
            Name = name;
            PurchasePrice = purchasePrice;
            SellingPrice = sellingPrice;
            Unit = unit;
            MinStock = minStock;
            IsActive = isActive;
            CreatedAt = createdAt;
            ImagePath = imagePath;
        }

        public void UpdateDetails(
            int categoryId, string name, decimal purchasePrice,
            decimal sellingPrice, string unit, int minStock)
        {
            ValidationHelper.EnsureNotEmpty(name, nameof(name));
            ValidationHelper.EnsureNotEmpty(unit, nameof(unit));
            ValidationHelper.EnsureNonNegative(purchasePrice, nameof(purchasePrice));
            ValidationHelper.EnsureNonNegative(sellingPrice, nameof(sellingPrice));
            if (minStock < 0)
            {
                throw new ArgumentException("Minimum stock cannot be negative.", nameof(minStock));
            }

            CategoryId = categoryId;
            Name = name;
            PurchasePrice = purchasePrice;
            SellingPrice = sellingPrice;
            Unit = unit;
            MinStock = minStock;
        }

        /// <summary>Null clears the photo (e.g. user removes it); otherwise stores the new relative path.</summary>
        public void SetImage(string? imagePath) => ImagePath = imagePath;

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}
