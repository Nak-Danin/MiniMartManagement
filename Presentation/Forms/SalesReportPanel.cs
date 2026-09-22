using MiniMartManagement.Models;
using MiniMartManagement.Services;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>Embedded (not a popup) sales-reports page - hosted directly in DashboardForm's content area.</summary>
    public class SalesReportPanel : UserControl
    {
        private readonly AppServices _services;
        private readonly User _currentUser;

        // Sales-by-date tab
        private readonly DateTimePicker _fromPicker = new();
        private readonly DateTimePicker _toPicker = new();
        private readonly DataGridView _salesGrid = new();
        private readonly Label _totalLabel = new();

        // Best sellers / low stock / out of stock tabs
        private readonly DataGridView _bestSellersGrid = new();
        private readonly DataGridView _lowStockGrid = new();
        private readonly DataGridView _outOfStockGrid = new();

        public SalesReportPanel(AppServices services, User currentUser)
        {
            _services = services;
            _currentUser = currentUser;
            Dock = DockStyle.Fill;
            BuildUi();
            RunDateRangeReport();
            LoadBestSellers();
            LoadLowStock();
            LoadOutOfStock();
        }

        private void BuildUi()
        {
            UiTheme.StyleControl(this);

            var tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildSalesByDateTab());
            tabs.TabPages.Add(BuildSimpleReportTab("Best Sellers", _bestSellersGrid,
                ("ProductName", "Product", 300), ("QuantitySold", "Quantity Sold", 150)));
            tabs.TabPages.Add(BuildSimpleReportTab("Low Stock", _lowStockGrid,
                ("Code", "Code", 100), ("ProductName", "Product", 250), ("Quantity", "Quantity", 100), ("MinStock", "Min Stock", 100)));
            tabs.TabPages.Add(BuildSimpleReportTab("Out of Stock", _outOfStockGrid,
                ("Code", "Code", 100), ("ProductName", "Product", 250), ("Quantity", "Quantity", 100)));

            Controls.Add(tabs);
        }

        private TabPage BuildSalesByDateTab()
        {
            var page = new TabPage("Sales by Date");

            // topPanel is tall enough (56px) to hold labels above the pickers with
            // positive Y coordinates - Step 8 placed these labels at Y=-5, which
            // clipped their top half against the panel edge.
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 56, Padding = new Padding(10, 8, 10, 8) };
            _fromPicker.Value = DateTime.Today.AddDays(-30);
            _fromPicker.Format = DateTimePickerFormat.Short;
            _fromPicker.Location = new Point(10, 26);
            _fromPicker.Size = new Size(110, 25);

            _toPicker.Value = DateTime.Today;
            _toPicker.Format = DateTimePickerFormat.Short;
            _toPicker.Location = new Point(130, 26);
            _toPicker.Size = new Size(110, 25);

            var runButton = new Button { Text = "Run Report", Location = new Point(250, 24), Size = new Size(100, 27) };
            UiTheme.StylePrimaryButton(runButton);
            runButton.Click += (_, _) => RunDateRangeReport();

            topPanel.Controls.AddRange(new Control[]
            {
                new Label { Text = "From", Location = new Point(10, 6), AutoSize = true },
                _fromPicker,
                new Label { Text = "To", Location = new Point(130, 6), AutoSize = true },
                _toPicker,
                runButton
            });

            _salesGrid.Dock = DockStyle.Fill;
            _salesGrid.ReadOnly = true;
            _salesGrid.AllowUserToAddRows = false;
            _salesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _salesGrid.AutoGenerateColumns = false;
            _salesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "SaleId", HeaderText = "Sale #", Width = 70 });
            _salesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "SaleDate", HeaderText = "Date", Width = 140 });
            _salesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "EmployeeId", HeaderText = "Employee ID", Width = 90 });
            _salesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subtotal", HeaderText = "Subtotal", Width = 90 });
            _salesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Discount", HeaderText = "Discount", Width = 90 });
            _salesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total", Width = 90 });
            UiTheme.StyleGrid(_salesGrid);

            _totalLabel.Dock = DockStyle.Bottom;
            _totalLabel.Height = 34;
            _totalLabel.TextAlign = ContentAlignment.MiddleRight;
            _totalLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _totalLabel.Padding = new Padding(0, 0, 15, 0);

            page.Controls.Add(_salesGrid);
            page.Controls.Add(_totalLabel);
            page.Controls.Add(topPanel);
            return page;
        }

        private static TabPage BuildSimpleReportTab(string title, DataGridView grid, params (string Name, string Header, int Width)[] columns)
        {
            var page = new TabPage(title);
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoGenerateColumns = false;
            foreach (var (name, header, width) in columns)
            {
                grid.Columns.Add(new DataGridViewTextBoxColumn { Name = name, HeaderText = header, Width = width });
            }
            UiTheme.StyleGrid(grid);
            page.Controls.Add(grid);
            return page;
        }

        private void RunDateRangeReport()
        {
            DateTime from = _fromPicker.Value.Date;
            DateTime to = _toPicker.Value.Date.AddDays(1); // inclusive of the whole "to" day

            List<Sale> sales = _services.Reports.GetSalesByDateRange(_currentUser, from, to);

            _salesGrid.Rows.Clear();
            foreach (Sale sale in sales)
            {
                _salesGrid.Rows.Add(
                    sale.Id, sale.SaleDate.ToString("yyyy-MM-dd HH:mm"), sale.EmployeeId,
                    sale.Subtotal.ToString("0.00"), sale.Discount.ToString("0.00"), sale.Total.ToString("0.00"));
            }

            decimal totalRevenue = sales.Sum(s => s.Total);
            _totalLabel.Text = $"Total Sales: {sales.Count}    Total Revenue: {totalRevenue:0.00}";
        }

        private void LoadBestSellers()
        {
            var bestSellers = _services.Reports.GetBestSellingProducts(_currentUser, topN: 10);
            _bestSellersGrid.Rows.Clear();
            foreach (var (product, quantitySold) in bestSellers)
            {
                _bestSellersGrid.Rows.Add(product.Name, quantitySold);
            }
        }

        private void LoadLowStock()
        {
            List<Inventory> lowStock = _services.Reports.GetLowStockProducts(_currentUser);
            Dictionary<int, Product> productsById = _services.Products.GetAll().ToDictionary(p => p.Id);

            _lowStockGrid.Rows.Clear();
            foreach (Inventory inventory in lowStock)
            {
                if (productsById.TryGetValue(inventory.ProductId, out Product? product))
                {
                    _lowStockGrid.Rows.Add(product.ProductCode, product.Name, inventory.Quantity, product.MinStock);
                }
            }
        }

        private void LoadOutOfStock()
        {
            List<Inventory> outOfStock = _services.Reports.GetOutOfStockProducts(_currentUser);
            Dictionary<int, Product> productsById = _services.Products.GetAll().ToDictionary(p => p.Id);

            _outOfStockGrid.Rows.Clear();
            foreach (Inventory inventory in outOfStock)
            {
                if (productsById.TryGetValue(inventory.ProductId, out Product? product))
                {
                    _outOfStockGrid.Rows.Add(product.ProductCode, product.Name, inventory.Quantity);
                }
            }
        }
    }
}
