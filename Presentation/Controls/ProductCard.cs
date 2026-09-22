using MiniMartManagement.Models;

namespace MiniMartManagement.Presentation.Controls
{
    /// <summary>
    /// One product tile in the ecommerce-style catalog grid (ProductManagementPanel /
    /// ProductViewPanel): photo, name, price, an optional status badge, and an
    /// optional row of action buttons. Built with a top-down FlowLayoutPanel instead
    /// of several stacked Dock=Top panels, which keeps the vertical order exactly
    /// equal to the order controls are added - no risk of a later addition silently
    /// landing above an earlier one.
    /// </summary>
    public class ProductCard : Panel
    {
        public FlowLayoutPanel ActionsPanel { get; }

        public ProductCard(Product product, string subtitle, string priceText, (string Text, Color Background, Color Foreground)? badge = null)
        {
            Size = new Size(206, 300);
            Margin = new Padding(8);
            BackColor = UiTheme.CardBackground;
            Tag = product;

            Paint += (_, e) =>
            {
                using var pen = new Pen(UiTheme.Border);
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            };

            var imageBox = new PictureBox
            {
                Dock = DockStyle.Top,
                Height = 140,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(249, 250, 251)
            };
            imageBox.Image = UiTheme.LoadProductImageOrPlaceholder(product.ImagePath, 140);

            var infoFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = false,
                Padding = new Padding(12, 10, 12, 8)
            };

            var nameLabel = new Label
            {
                Text = product.Name,
                Font = UiTheme.BoldFont,
                ForeColor = UiTheme.TextPrimary,
                AutoSize = false,
                Size = new Size(178, 34),
                AutoEllipsis = true
            };
            var subtitleLabel = new Label
            {
                Text = subtitle,
                Font = UiTheme.SmallFont,
                ForeColor = UiTheme.TextSecondary,
                AutoSize = false,
                Size = new Size(178, 18),
                AutoEllipsis = true
            };
            var priceLabel = new Label
            {
                Text = priceText,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = UiTheme.Primary,
                AutoSize = false,
                Size = new Size(178, 26),
                Margin = new Padding(0, 4, 0, 4)
            };

            infoFlow.Controls.Add(nameLabel);
            infoFlow.Controls.Add(subtitleLabel);
            infoFlow.Controls.Add(priceLabel);

            if (badge != null)
            {
                var badgeLabel = UiTheme.CreateBadge(badge.Value.Text, badge.Value.Background, badge.Value.Foreground);
                badgeLabel.Margin = new Padding(0, 0, 0, 8);
                infoFlow.Controls.Add(badgeLabel);
            }

            ActionsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                Size = new Size(178, 34)
            };
            infoFlow.Controls.Add(ActionsPanel);

            Controls.Add(infoFlow);
            Controls.Add(imageBox);
        }
    }
}
