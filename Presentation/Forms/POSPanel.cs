using MiniMartManagement.Models;
using MiniMartManagement.Services;
using MiniMartManagement.Services.Exceptions;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>
    /// Embedded (not a popup) POS screen - hosted directly in DashboardForm's
    /// content area. Follows the exact workflow from section 10 of the spec:
    /// search -> select -> quantity -> check stock -> add to cart -> total ->
    /// payment -> change -> complete -> save -> decrease inventory -> receipt.
    /// Every step that touches a business rule delegates to SaleService rather
    /// than checking anything here directly.
    ///
    /// Step 8's checkout panel used a fixed-height TableLayoutPanel, which
    /// clipped the Subtotal/Total/Change labels whenever their font didn't fit
    /// the row height the table divided evenly. This version uses a top-down
    /// FlowLayoutPanel where every row sizes itself to its own content, and the
    /// whole checkout block is docked to the bottom of the cart so it is never
    /// squeezed by the item grid above it.
    /// </summary>
    public class POSPanel : UserControl
    {
        private readonly AppServices _services;
        private readonly Employee _employee;

        private readonly TextBox _searchBox = new();
        private readonly DataGridView _productGrid = new();
        private readonly NumericUpDown _quantityBox = new() { Minimum = 1, Maximum = 10_000, Value = 1 };

        private readonly DataGridView _cartGrid = new();
        private readonly Label _subtotalLabel = new();
        private readonly NumericUpDown _discountBox = new() { DecimalPlaces = 2, Maximum = 1_000_000 };
        private readonly Label _totalLabel = new();
        private readonly NumericUpDown _paymentBox = new() { DecimalPlaces = 2, Maximum = 1_000_000 };
        private readonly Label _changeLabel = new();

        private Sale _currentSale;
        private Dictionary<int, Product> _productsById = new();

        public POSPanel(AppServices services, Employee employee)
        {
            _services = services;
            _employee = employee;
            _currentSale = _services.Sales.StartSale(_employee);
            Dock = DockStyle.Fill;
            BuildUi();
            LoadProducts();
            RefreshCart();
        }

        private void BuildUi()
        {
            UiTheme.StyleControl(this);

            // ---- Left: product search + list ----
            var leftPanel = new Panel { Dock = DockStyle.Left, Width = 460, Padding = new Padding(10) };

            var searchPanel = new Panel { Dock = DockStyle.Top, Height = 35 };
            _searchBox.Location = new Point(0, 5);
            _searchBox.Size = new Size(280, 25);
            var searchButton = new Button { Text = "Search", Location = new Point(290, 3), Size = new Size(80, 27) };
            UiTheme.StylePrimaryButton(searchButton);
            var clearButton = new Button { Text = "Clear", Location = new Point(375, 3), Size = new Size(70, 27) };
            UiTheme.StyleSecondaryButton(clearButton);
            searchButton.Click += (_, _) => LoadProducts(_searchBox.Text.Trim());
            clearButton.Click += (_, _) => { _searchBox.Clear(); LoadProducts(); };
            searchPanel.Controls.AddRange(new Control[] { _searchBox, searchButton, clearButton });

            _productGrid.Dock = DockStyle.Fill;
            _productGrid.ReadOnly = true;
            _productGrid.AllowUserToAddRows = false;
            _productGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _productGrid.MultiSelect = false;
            _productGrid.AutoGenerateColumns = false;
            _productGrid.Columns.Add(new DataGridViewImageColumn { Name = "Photo", HeaderText = "", Width = 44, ImageLayout = DataGridViewImageCellLayout.Zoom });
            _productGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Code", HeaderText = "Code", Width = 70 });
            _productGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Product", Width = 150 });
            _productGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "Price", Width = 70 });
            _productGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock", HeaderText = "Stock", Width = 60 });
            UiTheme.StyleGrid(_productGrid);
            _productGrid.RowTemplate.Height = 44; // tall enough for the 32x32 thumbnail

            var addPanel = new Panel { Dock = DockStyle.Bottom, Height = 45 };
            addPanel.Controls.Add(new Label { Text = "Qty", Location = new Point(0, 12), AutoSize = true });
            _quantityBox.Location = new Point(35, 8);
            _quantityBox.Size = new Size(70, 25);
            var addToCartButton = new Button { Text = "Add to Cart", Location = new Point(120, 6), Size = new Size(150, 30) };
            UiTheme.StylePrimaryButton(addToCartButton);
            addToCartButton.Click += AddToCartButton_Click;
            addPanel.Controls.AddRange(new Control[] { _quantityBox, addToCartButton });

            leftPanel.Controls.Add(_productGrid);
            leftPanel.Controls.Add(addPanel);
            leftPanel.Controls.Add(searchPanel);

            // ---- Right: cart + checkout ----
            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            var cartHeaderPanel = new Panel { Dock = DockStyle.Top, Height = 34 };
            var cartLabel = new Label { Text = "Cart", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
            cartHeaderPanel.Controls.Add(cartLabel);

            _cartGrid.Dock = DockStyle.Fill;
            _cartGrid.ReadOnly = true;
            _cartGrid.AllowUserToAddRows = false;
            _cartGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _cartGrid.MultiSelect = false;
            _cartGrid.AutoGenerateColumns = false;
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Product", Width = 170 });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Qty", Width = 55 });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "UnitPrice", HeaderText = "Unit Price", Width = 90 });
            _cartGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subtotal", HeaderText = "Subtotal", Width = 90 });
            UiTheme.StyleGrid(_cartGrid);

            // ---- Checkout block: everything below the cart grid, sized to its own
            // content (never squeezed) and docked to the bottom of rightPanel. ----
            var checkoutFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0, 10, 0, 0)
            };

            var removeButton = new Button { Text = "Remove Selected Item", Size = new Size(440, 30), Margin = new Padding(0, 0, 0, 10) };
            UiTheme.StyleSecondaryButton(removeButton);
            removeButton.Click += RemoveButton_Click;

            _subtotalLabel.Font = new Font("Segoe UI", 11);
            _subtotalLabel.AutoSize = false;
            _subtotalLabel.Size = new Size(440, 26);
            _subtotalLabel.Margin = new Padding(0, 0, 0, 6);

            var discountRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, Margin = new Padding(0, 0, 0, 10) };
            discountRow.Controls.Add(new Label { Text = "Discount", AutoSize = false, Size = new Size(70, 30), TextAlign = ContentAlignment.MiddleLeft });
            _discountBox.Size = new Size(110, 25);
            _discountBox.Margin = new Padding(3, 3, 10, 3);
            discountRow.Controls.Add(_discountBox);
            var applyDiscountButton = new Button { Text = "Apply Discount", Size = new Size(150, 28) };
            UiTheme.StyleSecondaryButton(applyDiscountButton);
            applyDiscountButton.Click += (_, _) => ApplyDiscount();
            discountRow.Controls.Add(applyDiscountButton);

            _totalLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            _totalLabel.ForeColor = UiTheme.Primary;
            _totalLabel.AutoSize = false;
            _totalLabel.Size = new Size(440, 32);
            _totalLabel.Margin = new Padding(0, 0, 0, 10);

            var paymentRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, Margin = new Padding(0, 0, 0, 6) };
            paymentRow.Controls.Add(new Label { Text = "Payment", AutoSize = false, Size = new Size(70, 30), TextAlign = ContentAlignment.MiddleLeft });
            _paymentBox.Size = new Size(150, 25);
            _paymentBox.Margin = new Padding(3, 3, 0, 3);
            _paymentBox.ValueChanged += (_, _) => RefreshTotals();
            paymentRow.Controls.Add(_paymentBox);

            _changeLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            _changeLabel.ForeColor = UiTheme.Success;
            _changeLabel.AutoSize = false;
            _changeLabel.Size = new Size(440, 26);
            _changeLabel.Margin = new Padding(0, 0, 0, 14);

            var completeSaleButton = new Button { Text = "Complete Sale", Size = new Size(440, 42), Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(0, 0, 0, 8) };
            UiTheme.StylePrimaryButton(completeSaleButton);
            completeSaleButton.Click += CompleteSaleButton_Click;

            var clearCartButton = new Button { Text = "Clear Cart", Size = new Size(440, 32) };
            UiTheme.StyleSecondaryButton(clearCartButton);
            clearCartButton.Click += (_, _) => { _currentSale = _services.Sales.StartSale(_employee); RefreshCart(); };

            checkoutFlow.Controls.Add(removeButton);
            checkoutFlow.Controls.Add(_subtotalLabel);
            checkoutFlow.Controls.Add(discountRow);
            checkoutFlow.Controls.Add(_totalLabel);
            checkoutFlow.Controls.Add(paymentRow);
            checkoutFlow.Controls.Add(_changeLabel);
            checkoutFlow.Controls.Add(completeSaleButton);
            checkoutFlow.Controls.Add(clearCartButton);

            rightPanel.Controls.Add(_cartGrid);
            rightPanel.Controls.Add(checkoutFlow);
            rightPanel.Controls.Add(cartHeaderPanel);

            Controls.Add(rightPanel);
            Controls.Add(leftPanel);
        }

        private void LoadProducts(string? keyword = null)
        {
            List<Product> products = (string.IsNullOrWhiteSpace(keyword)
                ? _services.Products.GetAll()
                : _services.Products.Search(keyword))
                .Where(p => p.IsActive)
                .ToList();

            _productsById = products.ToDictionary(p => p.Id);

            _productGrid.Rows.Clear();
            foreach (Product product in products)
            {
                Inventory? inventory = _services.Inventory.GetByProductId(product.Id);
                Image thumbnail = UiTheme.LoadProductImageOrPlaceholder(product.ImagePath, 32);
                int rowIndex = _productGrid.Rows.Add(
                    thumbnail, product.ProductCode, product.Name, product.SellingPrice.ToString("0.00"), inventory?.Quantity ?? 0);
                _productGrid.Rows[rowIndex].Tag = product;
            }
        }

        private void AddToCartButton_Click(object? sender, EventArgs e)
        {
            if (_productGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a product first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var product = (Product)_productGrid.SelectedRows[0].Tag!;

            try
            {
                _services.Sales.AddItemToCart(_currentSale, product.Id, (int)_quantityBox.Value);
                RefreshCart();
                LoadProducts(_searchBox.Text.Trim()); // stock shown on the left doesn't change in the DB yet, but keeps quantity box sane
            }
            catch (BusinessRuleException ex)
            {
                MessageBox.Show(ex.Message, "Could Not Add Item", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RemoveButton_Click(object? sender, EventArgs e)
        {
            if (_cartGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a cart item first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var item = (SaleItem)_cartGrid.SelectedRows[0].Tag!;
            _services.Sales.RemoveItemFromCart(_currentSale, item);
            RefreshCart();
        }

        private void ApplyDiscount()
        {
            try
            {
                _services.Sales.ApplyDiscount(_currentSale, _discountBox.Value);
                RefreshTotals();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Invalid Discount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CompleteSaleButton_Click(object? sender, EventArgs e)
        {
            if (_currentSale.Items.Count == 0)
            {
                MessageBox.Show("Add at least one item before completing the sale.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _services.Sales.SetPayment(_currentSale, _paymentBox.Value);
                _services.Sales.CompleteSale(_currentSale);
            }
            catch (Exception ex) when (ex is BusinessRuleException or InvalidOperationException)
            {
                MessageBox.Show(ex.Message, "Could Not Complete Sale", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var receipt = new ReceiptForm(_currentSale, _employee, _productsById);
            receipt.ShowDialog(FindForm());

            // Start a fresh sale and refresh stock levels shown on the left (they just decreased).
            _currentSale = _services.Sales.StartSale(_employee);
            _discountBox.Value = 0;
            _paymentBox.Value = 0;
            RefreshCart();
            LoadProducts();
        }

        private void RefreshCart()
        {
            _cartGrid.Rows.Clear();
            foreach (SaleItem item in _currentSale.Items)
            {
                string name = _productsById.TryGetValue(item.ProductId, out Product? product) ? product.Name : $"Product #{item.ProductId}";
                int rowIndex = _cartGrid.Rows.Add(name, item.Quantity, item.UnitPrice.ToString("0.00"), item.Subtotal.ToString("0.00"));
                _cartGrid.Rows[rowIndex].Tag = item;
            }

            RefreshTotals();
        }

        private void RefreshTotals()
        {
            _subtotalLabel.Text = $"Subtotal: {_currentSale.Subtotal:0.00}";
            _totalLabel.Text = $"Total: {_currentSale.Total:0.00}";
            _changeLabel.Text = $"Change: {Math.Max(0, _paymentBox.Value - _currentSale.Total):0.00}";
        }
    }
}
