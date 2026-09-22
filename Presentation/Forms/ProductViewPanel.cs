using MiniMartManagement.Models;
using MiniMartManagement.Presentation.Controls;
using MiniMartManagement.Services;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>
    /// Embedded (not a popup), read-only ecommerce-style product catalog for
    /// Employees: browse, search, check stock. No Add/Edit/Activate buttons -
    /// that's ProductManagementPanel, which requires CanManageProductsAndCategories()
    /// and is Admin-only.
    /// </summary>
    public class ProductViewPanel : UserControl
    {
        private readonly AppServices _services;
        private readonly TextBox _searchBox = new();
        private readonly FlowLayoutPanel _cardGrid = new();
        private readonly Label _emptyLabel = new();

        public ProductViewPanel(AppServices services)
        {
            _services = services;
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
            searchButton.Click += (_, _) => LoadProducts(_searchBox.Text.Trim());
            clearButton.Click += (_, _) => { _searchBox.Clear(); LoadProducts(); };
            topPanel.Controls.AddRange(new Control[] { _searchBox, searchButton, clearButton });

            _cardGrid.Dock = DockStyle.Fill;
            _cardGrid.AutoScroll = true;
            _cardGrid.FlowDirection = FlowDirection.LeftToRight;
            _cardGrid.WrapContents = true;
            _cardGrid.Padding = new Padding(12, 12, 0, 12);
            _cardGrid.BackColor = UiTheme.Background;

            _emptyLabel.Text = "No products found.";
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
            List<Product> products = (string.IsNullOrWhiteSpace(keyword)
                ? _services.Products.GetAll()
                : _services.Products.Search(keyword))
                .Where(p => p.IsActive) // Employees only need to see what's actually sellable.
                .ToList();

            _cardGrid.Controls.Clear();
            _emptyLabel.Visible = products.Count == 0;

            foreach (Product product in products)
            {
                Inventory? inventory = _services.Inventory.GetByProductId(product.Id);
                int quantity = inventory?.Quantity ?? 0;

                (string Text, Color Background, Color Foreground) badge = inventory == null || inventory.IsOutOfStock
                    ? ("Out of Stock", UiTheme.DangerLight, UiTheme.Danger)
                    : inventory.IsLowStock(product.MinStock)
                        ? ("Low Stock", UiTheme.WarningLight, UiTheme.Warning)
                        : ("In Stock", UiTheme.SuccessLight, UiTheme.Success);

                var card = new ProductCard(
                    product,
                    subtitle: product.ProductCode,
                    priceText: $"{product.SellingPrice:0.00}  ·  {quantity} {product.Unit}(s)",
                    badge: badge);

                card.ActionsPanel.Visible = false; // read-only catalog - nothing to act on here
                _cardGrid.Controls.Add(card);
            }
        }
    }
}
