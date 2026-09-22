using MiniMartManagement.Models;
using MiniMartManagement.Presentation.Controls;
using MiniMartManagement.Services;
using MiniMartManagement.Services.Exceptions;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>
    /// Embedded (not a popup) product-management page - hosted directly in
    /// DashboardForm's content area. Shown as an ecommerce-style photo grid
    /// (one card per product) rather than a plain data table, with a photo
    /// field on each product that Admin can set from ProductEditDialog.
    /// </summary>
    public class ProductManagementPanel : UserControl
    {
        private readonly AppServices _services;
        private readonly User _currentUser;
        private readonly TextBox _searchBox = new();
        private readonly FlowLayoutPanel _cardGrid = new();
        private readonly Label _emptyLabel = new();

        public ProductManagementPanel(AppServices services, User currentUser)
        {
            _services = services;
            _currentUser = currentUser;
            Dock = DockStyle.Fill;
            BuildUi();
            LoadProducts();
        }

        private void BuildUi()
        {
            UiTheme.StyleControl(this);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 52, Padding = new Padding(10, 10, 10, 10) };
            _searchBox.Location = new Point(0, 11);
            _searchBox.Size = new Size(260, 25);
            var searchButton = new Button { Text = "Search", Location = new Point(270, 9), Size = new Size(80, 27) };
            UiTheme.StylePrimaryButton(searchButton);
            var clearButton = new Button { Text = "Clear", Location = new Point(355, 9), Size = new Size(70, 27) };
            UiTheme.StyleSecondaryButton(clearButton);
            var addButton = new Button { Text = "+ Add Product", Location = new Point(445, 9), Size = new Size(130, 27) };
            UiTheme.StylePrimaryButton(addButton);
            searchButton.Click += (_, _) => LoadProducts(_searchBox.Text.Trim());
            clearButton.Click += (_, _) => { _searchBox.Clear(); LoadProducts(); };
            addButton.Click += AddButton_Click;
            topPanel.Controls.AddRange(new Control[] { _searchBox, searchButton, clearButton, addButton });

            _cardGrid.Dock = DockStyle.Fill;
            _cardGrid.AutoScroll = true;
            _cardGrid.FlowDirection = FlowDirection.LeftToRight;
            _cardGrid.WrapContents = true;
            _cardGrid.Padding = new Padding(12, 12, 0, 12);
            _cardGrid.BackColor = UiTheme.Background;

            _emptyLabel.Text = "No products found. Use \"Add Product\" to create your first one.";
            _emptyLabel.AutoSize = false;
            _emptyLabel.Dock = DockStyle.Fill;
            _emptyLabel.TextAlign = ContentAlignment.MiddleCenter;
            _emptyLabel.ForeColor = UiTheme.TextSecondary;
            _emptyLabel.Visible = false;

            Controls.Add(_cardGrid);
            Controls.Add(_emptyLabel);
            Controls.Add(topPanel);
        }

        private void LoadProducts(string? keyword = null)
        {
            List<Product> products = string.IsNullOrWhiteSpace(keyword)
                ? _services.Products.GetAll()
                : _services.Products.Search(keyword);

            Dictionary<int, string> categoryNames = _services.Categories.GetAll()
                .ToDictionary(c => c.Id, c => c.Name);
            Dictionary<int, Inventory> inventoryByProduct = _services.Inventory.GetAll()
                .ToDictionary(i => i.ProductId);

            _cardGrid.Controls.Clear();
            _emptyLabel.Visible = products.Count == 0;

            foreach (Product product in products)
            {
                string categoryName = categoryNames.TryGetValue(product.CategoryId, out var name) ? name : "(unknown)";
                inventoryByProduct.TryGetValue(product.Id, out Inventory? inventory);
                int stockQty = inventory?.Quantity ?? 0;

                (string, Color, Color) badge = product.IsActive
                    ? (stockQty <= 0 ? ("Out of Stock", UiTheme.DangerLight, UiTheme.Danger)
                        : inventory != null && inventory.IsLowStock(product.MinStock) ? ("Low Stock", UiTheme.WarningLight, UiTheme.Warning)
                        : ("Active", UiTheme.SuccessLight, UiTheme.Success))
                    : ("Inactive", Color.FromArgb(229, 231, 235), UiTheme.TextSecondary);

                var card = new ProductCard(
                    product,
                    subtitle: $"{product.ProductCode} · {categoryName}",
                    priceText: $"{product.SellingPrice:0.00}  ({stockQty} in stock)",
                    badge: badge);

                var editButton = new Button { Text = "Edit", Size = new Size(84, 28) };
                UiTheme.StyleSecondaryButton(editButton);
                editButton.Click += (_, _) => EditProduct(product);

                var toggleButton = new Button { Text = product.IsActive ? "Deactivate" : "Activate", Size = new Size(94, 28) };
                UiTheme.StyleSecondaryButton(toggleButton);
                toggleButton.Click += (_, _) => ToggleProduct(product);

                card.ActionsPanel.Controls.Add(editButton);
                card.ActionsPanel.Controls.Add(toggleButton);

                _cardGrid.Controls.Add(card);
            }
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            List<Category> categories = _services.Categories.GetAll();
            if (categories.Count == 0)
            {
                MessageBox.Show("Create a category first before adding products.", "No Categories", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dialog = new ProductEditDialog(categories);
            if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;

            try
            {
                _services.Products.AddProduct(
                    _currentUser, dialog.SelectedCategoryId, dialog.ProductCode, dialog.ProductName,
                    dialog.PurchasePrice, dialog.SellingPrice, dialog.Unit, dialog.MinStock, dialog.ImagePath);
                LoadProducts();
            }
            catch (Exception ex) when (ex is BusinessRuleException or ArgumentException or UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message, "Could Not Add Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void EditProduct(Product product)
        {
            List<Category> categories = _services.Categories.GetAll();
            using var dialog = new ProductEditDialog(categories, product);
            if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;

            try
            {
                _services.Products.UpdateProduct(
                    _currentUser, product, dialog.SelectedCategoryId, dialog.ProductName,
                    dialog.PurchasePrice, dialog.SellingPrice, dialog.Unit, dialog.MinStock, dialog.ImagePath);
                LoadProducts();
            }
            catch (Exception ex) when (ex is BusinessRuleException or ArgumentException or UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message, "Could Not Update Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ToggleProduct(Product product)
        {
            bool newActive = !product.IsActive;
            try
            {
                _services.Products.SetProductActive(_currentUser, product.Id, newActive);
                LoadProducts();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
