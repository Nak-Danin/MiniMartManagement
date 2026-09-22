using MiniMartManagement.Models;
using MiniMartManagement.Services;
using MiniMartManagement.Services.Exceptions;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>Embedded (not a popup) inventory page - hosted directly in DashboardForm's content area.</summary>
    public class InventoryPanel : UserControl
    {
        private readonly AppServices _services;
        private readonly User _currentUser;
        private readonly DataGridView _grid = new();
        private readonly CheckBox _lowStockOnlyCheck = new();
        private readonly CheckBox _outOfStockOnlyCheck = new();

        public InventoryPanel(AppServices services, User currentUser)
        {
            _services = services;
            _currentUser = currentUser;
            Dock = DockStyle.Fill;
            BuildUi();
            LoadInventory();
        }

        private void BuildUi()
        {
            UiTheme.StyleControl(this);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(10, 8, 10, 8) };
            _lowStockOnlyCheck.Text = "Low stock only";
            _lowStockOnlyCheck.Location = new Point(10, 8);
            _lowStockOnlyCheck.AutoSize = true;
            _outOfStockOnlyCheck.Text = "Out of stock only";
            _outOfStockOnlyCheck.Location = new Point(140, 8);
            _outOfStockOnlyCheck.AutoSize = true;
            _lowStockOnlyCheck.CheckedChanged += (_, _) => { if (_lowStockOnlyCheck.Checked) _outOfStockOnlyCheck.Checked = false; LoadInventory(); };
            _outOfStockOnlyCheck.CheckedChanged += (_, _) => { if (_outOfStockOnlyCheck.Checked) _lowStockOnlyCheck.Checked = false; LoadInventory(); };
            topPanel.Controls.AddRange(new Control[] { _lowStockOnlyCheck, _outOfStockOnlyCheck });

            _grid.Dock = DockStyle.Fill;
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = false;
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Code", HeaderText = "Code", Width = 90 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Product Name", Width = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Quantity", Width = 80 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "MinStock", HeaderText = "Min Stock", Width = 80 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "LastUpdated", HeaderText = "Last Updated", Width = 130 });
            UiTheme.StyleGrid(_grid);

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 54, Padding = new Padding(10), BackColor = UiTheme.CardBackground };
            var adjustButton = new Button { Text = "Adjust Stock", Location = new Point(10, 11), Size = new Size(120, 30) };
            UiTheme.StylePrimaryButton(adjustButton);
            adjustButton.Click += AdjustButton_Click;
            bottomPanel.Controls.Add(adjustButton);

            Controls.Add(_grid);
            Controls.Add(bottomPanel);
            Controls.Add(topPanel);
        }

        private void LoadInventory()
        {
            List<Inventory> inventoryList;
            if (_lowStockOnlyCheck.Checked)
            {
                inventoryList = _services.Inventory.GetLowStock();
            }
            else if (_outOfStockOnlyCheck.Checked)
            {
                inventoryList = _services.Inventory.GetOutOfStock();
            }
            else
            {
                inventoryList = _services.Inventory.GetAll();
            }

            Dictionary<int, Product> productsById = _services.Products.GetAll().ToDictionary(p => p.Id);

            _grid.Rows.Clear();
            foreach (Inventory inventory in inventoryList)
            {
                if (!productsById.TryGetValue(inventory.ProductId, out Product? product))
                {
                    continue;
                }

                string status = inventory.IsOutOfStock ? "Out of Stock"
                    : inventory.IsLowStock(product.MinStock) ? "Low Stock"
                    : "OK";

                int rowIndex = _grid.Rows.Add(
                    product.ProductCode, product.Name, inventory.Quantity, product.MinStock,
                    status, inventory.LastUpdated.ToString("yyyy-MM-dd HH:mm"));

                _grid.Rows[rowIndex].Tag = (inventory, product);
                if (status == "Out of Stock")
                {
                    _grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Firebrick;
                }
                else if (status == "Low Stock")
                {
                    _grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkOrange;
                }
            }
        }

        private void AdjustButton_Click(object? sender, EventArgs e)
        {
            if (_grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a product first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var (inventory, product) = ((Inventory, Product))_grid.SelectedRows[0].Tag!;

            using var dialog = new StockAdjustDialog(product.Name, inventory.Quantity);
            if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;

            try
            {
                _services.Inventory.AdjustStock(_currentUser, product.Id, dialog.NewQuantity);
                LoadInventory();
            }
            catch (Exception ex) when (ex is BusinessRuleException or UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message, "Could Not Adjust Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
