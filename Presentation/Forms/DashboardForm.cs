using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using MiniMartManagement.Models;
using MiniMartManagement.Services;

namespace MiniMartManagement.Presentation.Forms
{
    public partial class DashboardForm : Form
    {
        private readonly AppServices? _services;
        private readonly User? _currentUser;

        // Added missing fields referenced by the runtime logic / navigation
        private readonly Dictionary<string, Button> _navButtons = new();
        private string? _activeKey;

        // store each nav button's default background so hover can restore it
        private readonly Dictionary<Button, Color> _defaultBackColors = new();

        // Parameterless ctor for the Designer. 
        public DashboardForm()
        {
            InitializeComponent();
            UiTheme.StyleForm(this);
        }

        // Runtime ctor used by Program.cs
        public DashboardForm(AppServices services, User currentUser) : this()
        {
            _services = services;
            _currentUser = currentUser;

            Text = _currentUser.GetDashboardTitle();
            BuildNavButtons();
            Navigate("Dashboard");
        }

        private void BuildNavButtons()
        {
            _sidebarFlowPanel.Controls.Clear();
            _navButtons.Clear();
            _defaultBackColors.Clear();

            void AddNav(string key, string text, EventHandler onClick)
            {
                var btn = new Button
                {
                    Text = text,
                    AutoSize = false,
                    Height = 40,
                    Width = 200,
                    Tag = key,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Margin = new Padding(4)
                };

                // Apply global theme first (keeps consistent sizing/fonts)
                UiTheme.StylePrimaryButton(btn);

                // Ensure local visual settings are applied and not ignored by visual styles
                btn.FlatStyle = FlatStyle.Flat;
                btn.UseVisualStyleBackColor = false;
                btn.FlatAppearance.BorderSize = 0;

                // Default (normal) state: Forecolor=Blue & Backcolor=White
                btn.BackColor = Color.White;
                btn.ForeColor = Color.Blue;

                // store default back color for this button so hover can restore it
                _defaultBackColors[btn] = btn.BackColor;

                // hover color (use theme primary so it matches app palette)
                var hoverBg = UiTheme.Primary;
                var hoverFg = Color.White;

                // hover behavior: blue background while hovered, revert on leave
                btn.MouseEnter += (s, e) =>
                {
                    // don't change appearance if button is currently active
                    if (_activeKey == (string?)btn.Tag) return;
                    btn.BackColor = hoverBg;
                    btn.ForeColor = hoverFg;
                };
                btn.MouseLeave += (s, e) =>
                {
                    // restore active or normal visuals
                    if (_activeKey == (string?)btn.Tag)
                    {
                        btn.BackColor = UiTheme.Primary;
                        btn.ForeColor = Color.White;
                    }
                    else if (_defaultBackColors.TryGetValue(btn, out var d))
                    {
                        btn.BackColor = d;
                        btn.ForeColor = Color.Blue;
                    }
                };

                btn.Click += onClick;
                _navButtons[key] = btn;
                _sidebarFlowPanel.Controls.Add(btn);
            }

            // Always include Dashboard
            AddNav("Dashboard", "Dashboard", (_, _) => Navigate("Dashboard"));

            if (_currentUser != null && _currentUser.CanManageEmployees())
                AddNav("Employees", "Employee Management", (_, _) => Navigate("Employees"));

            if (_currentUser != null && _currentUser.CanManageProductsAndCategories())
            {
                AddNav("Products", "Product Management", (_, _) => Navigate("Products"));
                AddNav("Categories", "Category Management", (_, _) => Navigate("Categories"));
            }

            if (_currentUser != null && _currentUser.CanManageInventory())
                AddNav("Inventory", "Inventory", (_, _) => Navigate("Inventory"));

            if (_currentUser != null && _currentUser.CanViewAllSalesReports())
                AddNav("Reports", "Sales Reports", (_, _) => Navigate("Reports"));

            // Employee-specific items (type-check via Role)
            if (_currentUser != null && _currentUser.Role == UserRole.Employee)
            {
                AddNav("POS", "POS", (_, _) => Navigate("POS"));
                AddNav("ProductView", "Product List", (_, _) => Navigate("ProductView"));
                AddNav("MySales", "My Sales", (_, _) => Navigate("MySales"));
            }

            // Logout
            var logout = new Button { Text = "Logout", AutoSize = false, Height = 40, Width = 200, Margin = new Padding(4) };
            UiTheme.StyleDangerButton(logout);
            logout.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };
            _sidebarFlowPanel.Controls.Add(logout);
        }

        public void Navigate(string key)
        {
            if (_activeKey == key) return;
            _activeKey = key;

            _pageTitleLabel.Text = key;
            _pageSubtitleLabel.Text = key switch
            {
                "Dashboard" => "Overview",
                "Employees" => "Manage employees",
                "Products" => "Manage products",
                "Categories" => "Manage categories",
                "Inventory" => "Inventory & stock",
                "Reports" => "Sales reports",
                "POS" => "Point of Sale",
                "ProductView" => "Product list",
                "MySales" => "Your sales",
                _ => string.Empty
            };

            _contentBody.Controls.Clear();

            // Try to create a real panel for this key (convention: {Key}Panel)
            var panel = CreatePanelForKey(key);
            if (panel != null)
            {
                panel.Dock = DockStyle.Fill;
                _contentBody.Controls.Add(panel);
            }
            else
            {
                // Fallback placeholder (what you've been seeing)
                var placeholder = new Label
                {
                    Text = $"{key} - coming soon",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = UiTheme.BaseFont
                };
                _contentBody.Controls.Add(placeholder);
            }

            // Highlight active nav button: set both BackColor and ForeColor
            foreach (var kv in _navButtons)
            {
                var btn = kv.Value;
                if (kv.Key == key)
                {
                    btn.BackColor = UiTheme.Primary;
                    btn.ForeColor = Color.White;             // active text color
                    btn.Font = new Font(btn.Font, FontStyle.Bold);
                }
                else
                {
                    // restore to stored default so hover can work consistently
                    if (_defaultBackColors.TryGetValue(btn, out var d))
                        btn.BackColor = d;
                    else
                        btn.BackColor = Color.White;

                    btn.ForeColor = Color.Blue;              // default text color
                    btn.Font = new Font(btn.Font, FontStyle.Regular);
                }
            }
        }

        // Missing field documented in the code comments of DashboardForm.Designer.cs
        private static readonly Dictionary<string, string> _panelNameMap = new()
        {
            ["Dashboard"] = "DashboardHomePanel",
            ["Employees"] = "EmployeeManagementPanel",
            ["Products"] = "ProductManagementPanel",
            ["Categories"] = "CategoryManagementPanel",
            ["Inventory"] = "InventoryPanel",
            ["Reports"] = "SalesReportPanel",
            ["POS"] = "POSPanel",
            ["ProductView"] = "ProductViewPanel",
            ["MySales"] = "MySalesPanel"
        };

        // Reflection-based factory: looks for a class named "{key}Panel" in this assembly
        // and tries to instantiate it using common constructor patterns:
        // (AppServices, User, Action<string>), (AppServices, User), (AppServices, Action<string>),
        // (AppServices), (Action<string>), parameterless.
        private Control? CreatePanelForKey(string key)
        {
            var asm = Assembly.GetExecutingAssembly();

            // build candidate type names to try (mapped name first, then sensible fallbacks)
            var candidates = new List<string>();
            if (_panelNameMap.TryGetValue(key, out var mapped)) candidates.Add(mapped);
            candidates.Add($"{key}Panel");
            candidates.Add($"{key}HomePanel");

            foreach (var name in candidates)
            {
                var typeName = $"MiniMartManagement.Presentation.Forms.{name}";
                var t = asm.GetType(typeName);
                if (t == null || !typeof(Control).IsAssignableFrom(t)) continue;

                foreach (var ctor in t.GetConstructors())
                {
                    var parameters = ctor.GetParameters();
                    var args = new List<object?>();
                    var ok = true;

                    foreach (var p in parameters)
                    {
                        if (p.ParameterType == typeof(AppServices))
                        {
                            if (_services == null) { ok = false; break; }
                            args.Add(_services);
                        }
                        else if (p.ParameterType == typeof(User))
                        {
                            if (_currentUser == null) { ok = false; break; }
                            args.Add(_currentUser);
                        }
                        else if (p.ParameterType == typeof(Action<string>))
                        {
                            args.Add((Action<string>)Navigate);
                        }
                        else
                        {
                            ok = false;
                            break;
                        }
                    }

                    if (!ok) continue;

                    try
                    {
                        var instance = ctor.Invoke(args.ToArray()) as Control;
                        if (instance != null) return instance;
                    }
                    catch
                    {
                        // constructor failed, try next
                    }
                }
            }

            return null;
        }
    }
}
