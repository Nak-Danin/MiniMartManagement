using System.Drawing.Printing;
using System.Text;
using MiniMartManagement.Models;

namespace MiniMartManagement.Presentation.Forms
{
    public partial class ReceiptForm : Form
    {
        private readonly Sale _sale;
        private readonly Employee _employee;
        private readonly IReadOnlyDictionary<int, Product> _productsById;
        private readonly PrintDocument _printDocument = new();

        // Param ctor
        public ReceiptForm(Sale sale, Employee employee, IReadOnlyDictionary<int, Product> productsById)
        {
            _sale = sale;
            _employee = employee;
            _productsById = productsById;

            InitializeComponent();

            _printDocument.PrintPage += PrintDocument_PrintPage;

            UiTheme.StyleForm(this);

            // Populate items grid
            foreach (SaleItem item in _sale.Items)
            {
                string productName = _productsById.TryGetValue(item.ProductId, out Product? product)
                    ? product.Name
                    : $"Product #{item.ProductId}";
                itemsGrid.Rows.Add(productName, item.Quantity, item.UnitPrice.ToString("0.00"), item.Subtotal.ToString("0.00"));
            }

            // Build totals rows dynamically into totalsPanel
            totalsPanel.Controls.Clear();
            totalsPanel.ColumnCount = 2;
            totalsPanel.ColumnStyles.Clear();
            totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            totalsPanel.RowCount = 0;

            AddTotalsRow(totalsPanel, "Subtotal", _sale.Subtotal, UiTheme.BaseFont, UiTheme.TextPrimary);
            AddTotalsRow(totalsPanel, "Discount", _sale.Discount, UiTheme.BaseFont, UiTheme.TextPrimary);

            // separator row
            totalsPanel.RowCount++;
            var sep = new Panel { Height = 1, BackColor = UiTheme.Border, Margin = new Padding(0, 8, 0, 8), Dock = DockStyle.Fill };
            totalsPanel.Controls.Add(sep, 0, totalsPanel.RowCount - 1);
            totalsPanel.SetColumnSpan(sep, 2);

            AddTotalsRow(totalsPanel, "Total", _sale.Total, new Font("Segoe UI", 13, FontStyle.Bold), UiTheme.Primary, rowHeight: 34);
            AddTotalsRow(totalsPanel, "Payment", _sale.Payment, UiTheme.BaseFont, UiTheme.TextPrimary);
            AddTotalsRow(totalsPanel, "Change", _sale.ChangeAmount, UiTheme.BoldFont, UiTheme.Success);

            // Wire buttons
            printButton.Click += (_, _) => _printDocument.Print();
            closeButton.Click += (_, _) => Close();
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
