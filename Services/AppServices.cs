using MiniMartManagement.Database;
using MiniMartManagement.Interfaces;
using MiniMartManagement.Repositories;

namespace MiniMartManagement.Services
{
    /// <summary>
    /// Composition root: builds the connection factory, every repository,
    /// and every service exactly once, then hands the services out to
    /// whichever Form needs them. This is deliberately a plain class
    /// instead of a full DI container - one extra abstraction the project
    /// doesn't need yet (see section 18: "do not over-engineer").
    /// </summary>
    public class AppServices
    {
        public AuthService Auth { get; }
        public EmployeeService Employees { get; }
        public CategoryService Categories { get; }
        public ProductService Products { get; }
        public InventoryService Inventory { get; }
        public SaleService Sales { get; }
        public ReportService Reports { get; }

        public AppServices()
        {
            var connectionFactory = new DbConnectionFactory();

            IUserRepository userRepository = new UserRepository(connectionFactory);
            IEmployeeRepository employeeRepository = new EmployeeRepository(connectionFactory);
            ICategoryRepository categoryRepository = new CategoryRepository(connectionFactory);
            IProductRepository productRepository = new ProductRepository(connectionFactory);
            IInventoryRepository inventoryRepository = new InventoryRepository(connectionFactory);
            ISaleRepository saleRepository = new SaleRepository(connectionFactory);

            Auth = new AuthService(userRepository);
            Employees = new EmployeeService(employeeRepository, userRepository);
            Categories = new CategoryService(categoryRepository);
            Products = new ProductService(productRepository, categoryRepository, inventoryRepository);
            Inventory = new InventoryService(inventoryRepository);
            Sales = new SaleService(saleRepository, productRepository, inventoryRepository);
            Reports = new ReportService(saleRepository, inventoryRepository, productRepository);
        }
    }
}
