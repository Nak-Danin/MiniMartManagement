using MiniMartManagement.Models;
using MiniMartManagement.Services;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>
    /// Shown modally after a successful login. The sidebar buttons shown
    /// depend entirely on what currentUser.CanXxx() returns - the same
    /// polymorphic checks EmployeeService/ProductService/etc. use to
    /// authorize actions in Step 5 also decide what the UI even offers.
    ///
    /// Unlike Step 8 (where every sidebar button opened its screen in a
    /// separate popup window via ShowDialog), each sidebar item here swaps
    /// the corresponding page directly into the content area on the right,
    /// next to the sidebar, with no popup involved.
    /// </summary>
    public class DashboardForm : Form
    {
        private readonly AppServices _services;
        private readonly User _currentUser;

        private readonly Dictionary<string, Button> _navButtons = new();
        private readonly Panel _contentBody = new();
        private readonly Label _pageTitleLabel = new();
        private readonly Label _pageSubtitleLabel = new();

        private string? _activeKey;

        public DashboardForm(AppServices services, User currentUser)
        {
            _services = services;
            _currentUser = currentUser;
            BuildUi();
            Navigate("Dashboard");
        }

        private void BuildUi()
        {
            UiTheme.StyleForm(this);
            Text = _currentUser.GetDashboardTitle();
            ClientSize = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(860, 560);
            WindowState = FormWindowState.Maximized;

            Controls.Add(BuildContentArea());
            Controls.Add(BuildSidebar());
        }

        private Panel BuildSidebar()
        {
            var sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 240,
                BackColor = UiTheme.SidebarBackground,
                Padding = new Padding(0)
            };

            string displayName = _currentUser is Employee employee ? employee.FullName : _currentUser.Username;

            // Top-down flow keeps brand -> profile -> separator -> nav items in exactly
            // the order they're added, with no risk of a later item landing "above" an
            // earlier one (which is what Dock=Top stacking is prone to if you're not
            // careful about add order).
            var topFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true
            };

            var brandLabel = new Label
            {
                Text = "MiniMart",
                AutoSize = false,
                Size = new Size(240, 60),
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };

            var profileLabel = new Label
            {
                Text = $"{displayName}\n{_currentUser.Role}",
                AutoSize = false,
                Size = new Size(240, 54),
                Font = UiTheme.BaseFont,
                ForeColor = UiTheme.SidebarSubtleText,
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(20, 4, 0, 0)
            };

            var separator = new Panel { Size = new Size(240, 1), BackColor = UiTheme.SidebarBackgroundHover, Margin = new Padding(0, 8, 0, 8) };

            var navPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(10, 4, 10, 0)
            };

            navPanel.Controls.Add(CreateSidebarButton("Dashboard", "Dashboard"));
            foreach (string label in GetMenuItemsForCurrentRole())
            {
                navPanel.Controls.Add(CreateSidebarButton(label, label));
            }

            topFlow.Controls.Add(brandLabel);
            topFlow.Controls.Add(profileLabel);
            topFlow.Controls.Add(separator);
            topFlow.Controls.Add(navPanel);

            var logoutButton = new Button
            {
                Text = "Logout",
                Dock = DockStyle.Bottom,
                Height = 46,
                FlatStyle = FlatStyle.Flat,
                BackColor = UiTheme.SidebarBackground,
                ForeColor = Color.White,
                Font = UiTheme.BoldFont,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.FlatAppearance.MouseOverBackColor = UiTheme.Danger;
            logoutButton.Click += LogoutButton_Click;

            sidebar.Controls.Add(topFlow);
            sidebar.Controls.Add(logoutButton);

            return sidebar;
        }

        private Button CreateSidebarButton(string key, string label)
        {
            var button = new Button
            {
                Text = "   " + label,
                Size = new Size(210, 42),
                Margin = new Padding(0, 2, 0, 2),
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                BackColor = UiTheme.SidebarBackground,
                ForeColor = UiTheme.SidebarText,
                Font = UiTheme.BaseFont,
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = UiTheme.SidebarBackgroundHover;
            button.Click += (_, _) => Navigate(key);
            _navButtons[key] = button;
            return button;
        }

        private Panel BuildContentArea()
        {
            var contentArea = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Background };

            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = UiTheme.CardBackground, Padding = new Padding(28, 0, 28, 0) };
            headerPanel.Paint += (_, e) =>
            {
                using var pen = new Pen(UiTheme.Border);
                e.Graphics.DrawLine(pen, 0, headerPanel.Height - 1, headerPanel.Width, headerPanel.Height - 1);
            };

            var headerFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = false };
            _pageTitleLabel.AutoSize = false;
            _pageTitleLabel.Size = new Size(600, 26);
            _pageTitleLabel.Font = UiTheme.SubheadingFont;
            _pageTitleLabel.ForeColor = UiTheme.TextPrimary;
            _pageTitleLabel.Margin = new Padding(0, 10, 0, 0);
            _pageSubtitleLabel.AutoSize = false;
            _pageSubtitleLabel.Size = new Size(600, 18);
            _pageSubtitleLabel.Font = UiTheme.SmallFont;
            _pageSubtitleLabel.ForeColor = UiTheme.TextSecondary;
            headerFlow.Controls.Add(_pageTitleLabel);
            headerFlow.Controls.Add(_pageSubtitleLabel);
            headerPanel.Controls.Add(headerFlow);

            _contentBody.Dock = DockStyle.Fill;
            _contentBody.BackColor = UiTheme.Background;

            contentArea.Controls.Add(_contentBody);
            contentArea.Controls.Add(headerPanel);

            return contentArea;
        }

        /// <summary>
        /// Menu items are driven by the same permission checks the Service
        /// layer uses - an Employee's CanManageEmployees()/CanManageProductsAndCategories()/
        /// etc. all return false, so none of the Admin buttons even appear.
        /// </summary>
        private List<string> GetMenuItemsForCurrentRole()
        {
            var items = new List<string>();

            if (_currentUser is Employee)
            {
                items.Add("Point of Sale");
            }

            if (_currentUser.CanManageEmployees())
            {
                items.Add("Employee Management");
            }

            if (_currentUser.CanManageProductsAndCategories())
            {
                items.Add("Category Management");
                items.Add("Product Management");
            }

            if (_currentUser.CanManageInventory())
            {
                items.Add("Inventory");
            }

            if (_currentUser.CanViewAllSalesReports())
            {
                items.Add("Sales Reports");
            }

            if (_currentUser is Employee)
            {
                items.Add("Product List");
                items.Add("My Sales History");
            }

            return items;
        }

        private static string GetPageSubtitle(string key) => key switch
        {
            "Dashboard" => "Overview and quick actions",
            "Point of Sale" => "Ring up a new sale",
            "Employee Management" => "Add, edit, and manage staff accounts",
            "Category Management" => "Organize products into categories",
            "Product Management" => "Add, edit, and manage the product catalog",
            "Inventory" => "Track stock levels across all products",
            "Sales Reports" => "Revenue, best sellers, and stock alerts",
            "Product List" => "Browse what's available to sell",
            "My Sales History" => "Sales you've personally processed",
            _ => string.Empty
        };

        private void Navigate(string key)
        {
            if (_activeKey == key) return;

            UserControl? page = key switch
            {
                "Dashboard" => new DashboardHomePanel(_services, _currentUser, Navigate),
                "Employee Management" => new EmployeeManagementPanel(_services, _currentUser),
                "Category Management" => new CategoryManagementPanel(_services, _currentUser),
                "Product Management" => new ProductManagementPanel(_services, _currentUser),
                "Inventory" => new InventoryPanel(_services, _currentUser),
                "Sales Reports" => new SalesReportPanel(_services, _currentUser),
                "Point of Sale" => _currentUser is Employee posEmployee ? new POSPanel(_services, posEmployee) : null,
                "Product List" => new ProductViewPanel(_services),
                "My Sales History" => _currentUser is Employee salesEmployee ? new MySalesPanel(_services, salesEmployee) : null,
                _ => null
            };

            if (page == null) return;

            // Swap the content: dispose the outgoing page (releases its grid/image
            // handles) only after the new one is safely in place.
            Control? previous = _contentBody.Controls.Count > 0 ? _contentBody.Controls[0] : null;

            page.Dock = DockStyle.Fill;
            _contentBody.Controls.Add(page);
            previous?.Dispose();

            _pageTitleLabel.Text = key;
            _pageSubtitleLabel.Text = GetPageSubtitle(key);

            if (_activeKey != null && _navButtons.TryGetValue(_activeKey, out Button? previousButton))
            {
                previousButton.BackColor = UiTheme.SidebarBackground;
                previousButton.ForeColor = UiTheme.SidebarText;
                previousButton.Font = UiTheme.BaseFont;
            }

            if (_navButtons.TryGetValue(key, out Button? activeButton))
            {
                activeButton.BackColor = UiTheme.SidebarBackgroundActive;
                activeButton.ForeColor = Color.White;
                activeButton.Font = UiTheme.BoldFont;
            }

            _activeKey = key;
        }

        private void LogoutButton_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
