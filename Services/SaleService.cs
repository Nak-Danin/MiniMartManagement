using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;
using MiniMartManagement.Services.Exceptions;
using MiniMartManagement.Utilities;

namespace MiniMartManagement.Services
{
    public class SaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public SaleService(
            ISaleRepository saleRepository,
            IProductRepository productRepository,
            IInventoryRepository inventoryRepository)
        {
            _saleRepository = saleRepository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
        }

        // Note: this takes an Employee, not a User - Sales.employee_id can only ever
        // reference an Employees row, so an Admin (who has no EmployeeId) physically
        // cannot start a sale. The database schema itself enforces "employees process sales".
        public Sale StartSale(Employee employee) => new Sale(employee.EmployeeId, DateTime.Now);

        /// <summary>Business rules 4, 5, 14: product must be active and have enough stock before it can be added to the cart.</summary>
        public void AddItemToCart(Sale sale, int productId, int quantity)
        {
            ValidationHelper.EnsurePositive(quantity, nameof(quantity));

            Product? product = _productRepository.GetById(productId);
            if (product == null)
            {
                throw new BusinessRuleException("Product not found.");
            }

            if (!product.IsActive)
            {
                throw new BusinessRuleException($"'{product.Name}' is inactive and cannot be sold.");
            }

            Inventory? inventory = _inventoryRepository.GetByProductId(productId);
            if (inventory == null || !inventory.HasSufficientStock(quantity))
            {
                int available = inventory?.Quantity ?? 0;
                throw new BusinessRuleException(
                    $"Insufficient stock for '{product.Name}'. Requested {quantity}, only {available} available.");
            }

            sale.AddItem(new SaleItem(product.Id, quantity, product.SellingPrice));
        }

        public void RemoveItemFromCart(Sale sale, SaleItem item) => sale.RemoveItem(item);

        public void ApplyDiscount(Sale sale, decimal discount) => sale.ApplyDiscount(discount);

        /// <summary>Business rule 9/10: payment must cover the total; change is computed automatically by Sale.ChangeAmount.</summary>
        public void SetPayment(Sale sale, decimal payment) => sale.SetPayment(payment);

        /// <summary>Business rule 15: Sale + SaleItems + stock decrease happen atomically inside SaleRepository.</summary>
        public void CompleteSale(Sale sale)
        {
            sale.EnsureCanComplete();

            try
            {
                _saleRepository.CompleteSale(sale);
            }
            catch (InvalidOperationException ex)
            {
                // The repository already rolled back the transaction (e.g. stock ran out
                // between adding to cart and checkout). Re-thrown as a business-rule
                // failure so the UI can display the message directly.
                throw new BusinessRuleException(ex.Message);
            }
        }

        public List<Sale> GetSalesForEmployee(Employee employee) => _saleRepository.GetByEmployeeId(employee.EmployeeId);

        public Sale? GetSaleById(int saleId) => _saleRepository.GetById(saleId);
    }
}
