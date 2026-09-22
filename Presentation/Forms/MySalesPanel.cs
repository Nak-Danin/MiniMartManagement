using MiniMartManagement.Models;
using MiniMartManagement.Services;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>
    /// Embedded (not a popup) sales-history page for the signed-in Employee.
    /// GetSalesForEmployee is scoped to _employee, so there's no way to see
    /// anyone else's sales from here.
    /// </summary>
    public class MySalesPanel : UserControl
    {
        private readonly AppServices _services;
        private readonly Employee _employee;
        private readonly DataGridView _grid = new();

        public MySalesPanel(AppServices services, Employee employee)
        {
            _services = services;
            _employee = employee;
            Dock = DockStyle.Fill;
            BuildUi();
            LoadSales();
        }

        private void BuildUi()
        {
            UiTheme.StyleControl(this);

            var headerLabel = new Label
            {
                Text = $"Sales history for {_employee.FullName}",
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(15, 12, 0, 0),
                Font = UiTheme.SubheadingFont,
                ForeColor = UiTheme.TextPrimary
            };

            UiTheme.StyleGrid(_grid);
            _grid.Dock = DockStyle.Fill;
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = false;
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "SaleId", HeaderText = "Sale #", Width = 70 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "SaleDate", HeaderText = "Date", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Items", HeaderText = "Items", Width = 70 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subtotal", HeaderText = "Subtotal", Width = 90 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Discount", HeaderText = "Discount", Width = 90 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total", Width = 90 });

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 58, Padding = new Padding(15, 12, 15, 12), BackColor = UiTheme.CardBackground };
            var viewReceiptButton = new Button { Text = "View Receipt", Location = new Point(0, 9), Size = new Size(140, 32) };
            UiTheme.StylePrimaryButton(viewReceiptButton);
            viewReceiptButton.Click += ViewReceiptButton_Click;
            bottomPanel.Controls.Add(viewReceiptButton);

            Controls.Add(_grid);
            Controls.Add(bottomPanel);
            Controls.Add(headerLabel);
        }

        private void LoadSales()
        {
            List<Sale> sales = _services.Sales.GetSalesForEmployee(_employee);

            _grid.Rows.Clear();
            foreach (Sale sale in sales)
            {
                int rowIndex = _grid.Rows.Add(
                    sale.Id, sale.SaleDate.ToString("yyyy-MM-dd HH:mm"), sale.Items.Count,
                    sale.Subtotal.ToString("0.00"), sale.Discount.ToString("0.00"), sale.Total.ToString("0.00"));
                _grid.Rows[rowIndex].Tag = sale;
            }
        }

        private void ViewReceiptButton_Click(object? sender, EventArgs e)
        {
            if (_grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a sale first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var sale = (Sale)_grid.SelectedRows[0].Tag!;
            Dictionary<int, Product> productsById = _services.Products.GetAll().ToDictionary(p => p.Id);

            using var receipt = new ReceiptForm(sale, _employee, productsById);
            receipt.ShowDialog(FindForm());
        }
    }
}
