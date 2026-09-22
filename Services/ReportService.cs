using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;

namespace MiniMartManagement.Services
{
    public class ReportService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;

        public ReportService(
            ISaleRepository saleRepository,
            IInventoryRepository inventoryRepository,
            IProductRepository productRepository)
        {
            _saleRepository = saleRepository;
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
        }

        public List<Sale> GetAllSales(User actingUser)
        {
            EnsureCanViewReports(actingUser);
            return _saleRepository.GetAll();
        }

        public List<Sale> GetSalesByDateRange(User actingUser, DateTime from, DateTime to)
        {
            EnsureCanViewReports(actingUser);
            return _saleRepository.GetByDateRange(from, to);
        }

        public List<Sale> GetSalesByEmployee(User actingUser, int employeeId)
        {
            EnsureCanViewReports(actingUser);
            return _saleRepository.GetByEmployeeId(employeeId);
        }

        public decimal GetTotalSales(User actingUser, DateTime from, DateTime to)
        {
            EnsureCanViewReports(actingUser);
            return _saleRepository.GetByDateRange(from, to).Sum(sale => sale.Total);
        }

        /// <summary>Aggregates quantity sold per product across all sales. Simple in-memory grouping - fine at MiniMart's scale.</summary>
        public List<(Product Product, int QuantitySold)> GetBestSellingProducts(User actingUser, int topN = 5)
        {
            EnsureCanViewReports(actingUser);

            var productsById = _productRepository.GetAll().ToDictionary(p => p.Id);

            var topSellers = _saleRepository.GetAll()
                .SelectMany(sale => sale.Items)
                .GroupBy(item => item.ProductId)
                .Select(group => (ProductId: group.Key, QuantitySold: group.Sum(item => item.Quantity)))
                .OrderByDescending(x => x.QuantitySold)
                .Take(topN);

            var results = new List<(Product, int)>();
            foreach (var (productId, quantitySold) in topSellers)
            {
                if (productsById.TryGetValue(productId, out var product))
                {
                    results.Add((product, quantitySold));
                }
            }

            return results;
        }

        public List<Inventory> GetLowStockProducts(User actingUser)
        {
            EnsureCanViewReports(actingUser);
            return _inventoryRepository.GetLowStock();
        }

        public List<Inventory> GetOutOfStockProducts(User actingUser)
        {
            EnsureCanViewReports(actingUser);
            return _inventoryRepository.GetOutOfStock();
        }

        private static void EnsureCanViewReports(User actingUser)
        {
            if (!actingUser.CanViewAllSalesReports())
            {
                throw new UnauthorizedAccessException("Only an Admin can view sales reports.");
            }
        }
    }
}
