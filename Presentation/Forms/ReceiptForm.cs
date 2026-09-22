using System.Drawing.Printing;
using System.Text;
using MiniMartManagement.Models;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>
    /// Shows a completed Sale as a proper itemized receipt, with a Print button
    /// wired to System.Drawing.Printing.PrintDocument. Used both right after
    /// checkout (POSPanel) and when reviewing past sales (MySalesPanel).
    ///
    /// Step 8 rendered this as a single fixed-size monospace TextBox, which cut
    /// off the totals section (Subtotal in particular) whenever the item list was
    /// long or the window couldn't be resized. This version keeps the item list
    /// and the totals in two separate areas: the item list scrolls on its own if
    /// there are many items, while Subtotal/Discount/Total/Payment/Change live in
    /// a fixed panel docked to the bottom that is never scrolled out of view.
    /// </summary>
    public class ReceiptForm : Form
    {
        private readonly Sale _sale;
        private readonly Employee _employee;
        private readonly IReadOnlyDictionary<int, Product> _productsById;
        private readonly PrintDocument _printDocument = new();

        public ReceiptForm(Sale sale, Employee employee, IReadOnlyDictionary<int, Product> productsById)
        {
            _sale = sale;
            _employee = employee;
            _productsById = productsById;
            _printDocument.PrintPage += PrintDocument_PrintPage;
            BuildUi();
        }

        private void BuildUi()
        {
            UiTheme.StyleForm(this);
            Text = "Receipt";
            ClientSize = new Size(420, 640);
            MinimumSize = new Size(400, 460);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = false;

            // ---- Header: store name + sale metadata ----
            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 108, BackColor = UiTheme.SidebarBackground, Padding = new Padding(20, 14, 20, 14) };
            var storeLabel = new Label
            {
                Text = "MiniMart",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.White
            };
            var metaLabel = new Label
            {
                Text = $"Sale #{_sale.Id}    {_sale.SaleDate:yyyy-MM-dd HH:mm}\nCashier: {_employee.FullName}",
                Dock = DockStyle.Top,
                Height = 50,
                Font = UiTheme.BaseFont,
                ForeColor = UiTheme.SidebarSubtleText
            };
            headerPanel.Controls.Add(metaLabel);
            headerPanel.Controls.Add(storeLabel);

            // ---- Totals: always fully visible, docked to the bottom, above the buttons ----
            var totalsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 200, // fixed and generous on purpose - this is the section Step 8 was clipping, so it must never depend on leftover space
                ColumnCount = 2,
                Padding = new Padding(20, 14, 20, 14),
                BackColor = Color.FromArgb(250, 250, 252)
            };
            totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            AddTotalsRow(totalsPanel, "Subtotal", _sale.Subtotal, UiTheme.BaseFont, UiTheme.TextPrimary);
            AddTotalsRow(totalsPanel, "Discount", _sale.Discount, UiTheme.BaseFont, UiTheme.TextPrimary);

            var separator = new Panel { Height = 1, BackColor = UiTheme.Border, Margin = new Padding(0, 8, 0, 8) };
            totalsPanel.Controls.Add(separator, 0, totalsPanel.RowCount);
            totalsPanel.SetColumnSpan(separator, 2);
            totalsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 17f)); // 1px line + its 8px top/bottom margin
            totalsPanel.RowCount++;

            AddTotalsRow(totalsPanel, "Total", _sale.Total, new Font("Segoe UI", 13, FontStyle.Bold), UiTheme.Primary, rowHeight: 34);
            AddTotalsRow(totalsPanel, "Payment", _sale.Payment, UiTheme.BaseFont, UiTheme.TextPrimary);
            AddTotalsRow(totalsPanel, "Change", _sale.ChangeAmount, UiTheme.BoldFont, UiTheme.Success);

            // ---- Buttons ----
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(20, 12, 20, 12),
                FlowDirection = FlowDirection.RightToLeft
            };
            var closeButton = new Button { Text = "Close", Size = new Size(110, 34) };
            UiTheme.StyleSecondaryButton(closeButton);
            closeButton.Click += (_, _) => Close();
            var printButton = new Button { Text = "Print", Size = new Size(110, 34), Margin = new Padding(0, 0, 10, 0) };
            UiTheme.StylePrimaryButton(printButton);
            printButton.Click += (_, _) => _printDocument.Print();
            buttonPanel.Controls.Add(closeButton);
            buttonPanel.Controls.Add(printButton);

            // ---- Item list: the only part that scrolls, so totals never get pushed off-screen ----
            var itemsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None
            };
            itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Product", HeaderText = "Product", Width = 180 });
            itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Qty", HeaderText = "Qty", Width = 45 });
            itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "UnitPrice", HeaderText = "Unit", Width = 75 });
            itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Subtotal", HeaderText = "Subtotal", Width = 75, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
            UiTheme.StyleGrid(itemsGrid);

            foreach (SaleItem item in _sale.Items)
            {
                string productName = _productsById.TryGetValue(item.ProductId, out Product? product)
                    ? product.Name
                    : $"Product #{item.ProductId}";
                itemsGrid.Rows.Add(productName, item.Quantity, item.UnitPrice.ToString("0.00"), item.Subtotal.ToString("0.00"));
            }

            var itemsWrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 16, 20, 8) };
            itemsWrapper.Controls.Add(itemsGrid);

            // Dock=Bottom add order matters when two controls share it: the control added
            // LAST claims the true outer edge. buttonPanel is added after totalsPanel so it
            // sits at the very bottom, with totalsPanel stacked directly above it - both are
            // fixed-height (not AutoSize), so neither can be squeezed to nothing by layout.
            Controls.Add(itemsWrapper);
            Controls.Add(totalsPanel);
            Controls.Add(buttonPanel);
            Controls.Add(headerPanel);
        }

        private static void AddTotalsRow(TableLayoutPanel panel, string label, decimal value, Font font, Color color, int rowHeight = 26)
        {
            int row = panel.RowCount;
            panel.RowCount++;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var labelControl = new Label
            {
                Text = label,
                Font = font,
                ForeColor = color,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Height = rowHeight,
                TextAlign = ContentAlignment.MiddleLeft
            };
            var valueControl = new Label
            {
                Text = value.ToString("0.00"),
                Font = font,
                ForeColor = color,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Height = rowHeight,
                TextAlign = ContentAlignment.MiddleRight
            };

            panel.Controls.Add(labelControl, 0, row);
            panel.Controls.Add(valueControl, 1, row);
        }

        private void PrintDocument_PrintPage(object? sender, PrintPageEventArgs e)
        {
            e.Graphics!.DrawString(BuildPrintText(), new Font("Consolas", 10), Brushes.Black, 20, 20);
        }

        private string BuildPrintText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("        MiniMart Management System");
            sb.AppendLine("------------------------------------------");
            sb.AppendLine($"Sale #   : {_sale.Id}");
            sb.AppendLine($"Date     : {_sale.SaleDate:yyyy-MM-dd HH:mm}");
            sb.AppendLine($"Cashier  : {_employee.FullName}");
            sb.AppendLine("------------------------------------------");

            foreach (SaleItem item in _sale.Items)
            {
                string productName = _productsById.TryGetValue(item.ProductId, out Product? product)
                    ? product.Name
                    : $"Product #{item.ProductId}";

                sb.AppendLine(productName.Length > 26 ? productName[..26] : productName);
                sb.AppendLine($"  {item.Quantity} x {item.UnitPrice,8:0.00} = {item.Subtotal,10:0.00}");
            }

            sb.AppendLine("------------------------------------------");
            sb.AppendLine($"{"Subtotal:",-20}{_sale.Subtotal,20:0.00}");
            sb.AppendLine($"{"Discount:",-20}{_sale.Discount,20:0.00}");
            sb.AppendLine($"{"Total:",-20}{_sale.Total,20:0.00}");
            sb.AppendLine($"{"Payment:",-20}{_sale.Payment,20:0.00}");
            sb.AppendLine($"{"Change:",-20}{_sale.ChangeAmount,20:0.00}");
            sb.AppendLine("------------------------------------------");
            sb.AppendLine("           Thank you!");

            return sb.ToString();
        }
    }
}
