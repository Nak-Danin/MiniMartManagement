using MiniMartManagement.Models;

namespace MiniMartManagement.Interfaces
{
    public interface ISaleRepository
    {
        /// <summary>
        /// Persists a completed sale: inserts the Sale row, its SaleItems,
        /// and decreases Inventory for each item - all inside a single
        /// database transaction (business rule 15). Sets sale.Id on success.
        /// Throws and leaves the database unchanged if anything fails.
        /// </summary>
        void CompleteSale(Sale sale);

        Sale? GetById(int saleId);
        List<Sale> GetByEmployeeId(int employeeId);
        List<Sale> GetByDateRange(DateTime from, DateTime to);
        List<Sale> GetAll();
    }
}
