using MiniMartManagement.Models;
using MiniMartManagement.Services;
using System.Globalization;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>
    /// The default content shown when the dashboard first loads (and whenever
    /// "Dashboard" is clicked in the sidebar) - a welcome message plus a few
    /// at-a-glance stat cards, scoped to what the signed-in user is actually
    /// allowed to see (same CanXxx() checks the rest of the app uses).
    /// </summary>
    public class DashboardHomePanel : UserControl
    {
        private readonly AppServices _services;
        private readonly User _currentUser;
        private readonly Action<string> _navigate;

        public DashboardHomePanel(AppServices services, User currentUser, Action<string> navigate)
        {
            _services = services;
            _currentUser = currentUser;
            _navigate = navigate;
            Dock = DockStyle.Fill;
            AutoScroll = true;
            BuildUi();
        }

        private void BuildUi()
        {
            UiTheme.StyleControl(this);

            var layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(24, 20, 24, 20)
            };

            string displayName = _currentUser is Employee employee ? employee.FullName : _currentUser.Username;

            var greetingLabel = new Label
            {
                Text = $"Welcome back, {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(displayName.ToLower())}",
                AutoSize = false,
                Size = new Size(760, 34),
                Font = UiTheme.HeadingFont,
                ForeColor = UiTheme.TextPrimary,
                Margin = new Padding(0, 0, 0, 4)
            };
            var subLabel = new Label
            {
                Text = $"You're signed in as {_currentUser.Role}.",
                AutoSize = false,
                Size = new Size(760, 24),
                Font = UiTheme.BaseFont,
                ForeColor = UiTheme.TextSecondary,
                Margin = new Padding(0, 0, 0, 20)
            };

            layout.Controls.Add(greetingLabel);
            layout.Controls.Add(subLabel);

            var statsRow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 24)
            };

            foreach ((string title, string value, Color accent) in BuildStats())
            {
                statsRow.Controls.Add(CreateStatCard(title, value, accent));
            }
            layout.Controls.Add(statsRow);

            var actionsHeader = new Label
            {
                Text = "Quick Actions",
                AutoSize = false,
                Size = new Size(760, 24),
                Font = UiTheme.SubheadingFont,
                ForeColor = UiTheme.TextPrimary,
                Margin = new Padding(0, 0, 0, 10)
            };
            layout.Controls.Add(actionsHeader);

            var actionsRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = true, AutoSize = true };
            foreach ((string label, string target) in BuildQuickActions())
            {
                var button = new Button { Text = label, Size = new Size(190, 40), Margin = new Padding(0, 0, 10, 10) };
                UiTheme.StylePrimaryButton(button);
                button.Click += (_, _) => _navigate(target);
                actionsRow.Controls.Add(button);
            }
            layout.Controls.Add(actionsRow);

            Controls.Add(layout);
        }

        private List<(string Title, string Value, Color Accent)> BuildStats()
        {
            var stats = new List<(string, string, Color)>();

            if (_currentUser.CanViewAllSalesReports())
            {
                decimal todayRevenue = _services.Reports.GetTotalSales(_currentUser, DateTime.Today, DateTime.Today.AddDays(1));
                stats.Add(("Today's Revenue", todayRevenue.ToString("0.00"), UiTheme.Primary));
            }

            if (_currentUser.CanManageProductsAndCategories())
            {
                stats.Add(("Products", _services.Products.GetAll().Count.ToString(), UiTheme.TextPrimary));
            }

            if (_currentUser.CanManageInventory())
            {
                stats.Add(("Low Stock", _services.Inventory.GetLowStock().Count.ToString(), UiTheme.Warning));
                stats.Add(("Out of Stock", _services.Inventory.GetOutOfStock().Count.ToString(), UiTheme.Danger));
            }

            if (_currentUser.CanManageEmployees())
            {
                stats.Add(("Employees", _services.Employees.GetAll(_currentUser).Count.ToString(), UiTheme.TextPrimary));
            }

            if (_currentUser is Employee employee)
            {
                var todaysSales = _services.Sales.GetSalesForEmployee(employee)
                    .Where(s => s.SaleDate.Date == DateTime.Today)
                    .ToList();
                stats.Add(("Your Sales Today", todaysSales.Count.ToString(), UiTheme.Primary));
                stats.Add(("Your Revenue Today", todaysSales.Sum(s => s.Total).ToString("0.00"), UiTheme.Success));
            }

            return stats;
        }

        private List<(string Label, string Target)> BuildQuickActions()
        {
            var actions = new List<(string, string)>();

            if (_currentUser is Employee)
            {
                actions.Add(("New Sale", "POS"));
                actions.Add(("Browse Products", "ProductView"));
                actions.Add(("My Sales History", "MySales"));
            }

            if (_currentUser.CanManageProductsAndCategories())
            {
                actions.Add(("Manage Products", "Products"));
            }

            if (_currentUser.CanManageInventory())
            {
                actions.Add(("Check Inventory", "Inventory"));
            }

            if (_currentUser.CanViewAllSalesReports())
            {
                actions.Add(("View Reports", "Reports"));
            }

            if (_currentUser.CanManageEmployees())
            {
                actions.Add(("Manage Employees", "Employees"));
            }

            return actions;
        }

        private void InitializeComponent()
        {

        }

        private static Panel CreateStatCard(string title, string value, Color accent)
        {
            var card = UiTheme.CreateCard();
            card.Size = new Size(190, 90);
            card.Margin = new Padding(0, 0, 12, 12);

            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = false };
            var valueLabel = new Label { Text = value, AutoSize = false, Size = new Size(158, 34), Font = new Font("Segoe UI", 17, FontStyle.Bold), ForeColor = accent };
            var titleLabel = new Label { Text = title, AutoSize = false, Size = new Size(158, 20), Font = UiTheme.SmallFont, ForeColor = UiTheme.TextSecondary };
            flow.Controls.Add(valueLabel);
            flow.Controls.Add(titleLabel);
            card.Controls.Add(flow);

            return card;
        }
    }
}
