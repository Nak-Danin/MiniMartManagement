namespace MiniMartManagement.Presentation
{
    /// <summary>
    /// Central color palette and styling helpers so every Form/DataGridView/
    /// Button looks consistent instead of default WinForms gray. Plain C#
    /// styling (FlatStyle, colors, fonts) - no external UI library, so it
    /// stays easy to explain in a report and has zero extra dependencies.
    /// </summary>
    public static class UiTheme
    {
        public static readonly Color Primary = Color.FromArgb(37, 99, 235);
        public static readonly Color PrimaryDark = Color.FromArgb(29, 78, 216);
        public static readonly Color PrimaryLight = Color.FromArgb(219, 234, 254);
        public static readonly Color Danger = Color.FromArgb(220, 38, 38);
        public static readonly Color DangerDark = Color.FromArgb(185, 28, 28);
        public static readonly Color DangerLight = Color.FromArgb(254, 226, 226);
        public static readonly Color Success = Color.FromArgb(22, 163, 74);
        public static readonly Color SuccessLight = Color.FromArgb(220, 252, 231);
        public static readonly Color Warning = Color.FromArgb(217, 119, 6);
        public static readonly Color WarningLight = Color.FromArgb(254, 243, 199);

        public static readonly Color Background = Color.FromArgb(244, 245, 248);
        public static readonly Color CardBackground = Color.White;
        public static readonly Color Border = Color.FromArgb(226, 229, 235);

        public static readonly Color SidebarBackground = Color.FromArgb(17, 24, 39);
        public static readonly Color SidebarBackgroundHover = Color.FromArgb(31, 41, 55);
        public static readonly Color SidebarBackgroundActive = Color.FromArgb(37, 99, 235);
        public static readonly Color SidebarText = Color.FromArgb(229, 231, 235);
        public static readonly Color SidebarSubtleText = Color.FromArgb(156, 163, 175);

        public static readonly Color TextPrimary = Color.FromArgb(31, 41, 55);
        public static readonly Color TextSecondary = Color.FromArgb(107, 114, 128);

        public static readonly Font BaseFont = new("Segoe UI", 9.5f);
        public static readonly Font BoldFont = new("Segoe UI", 9.5f, FontStyle.Bold);
        public static readonly Font SmallFont = new("Segoe UI", 8.5f);
        public static readonly Font SmallBoldFont = new("Segoe UI", 8.5f, FontStyle.Bold);
        public static readonly Font HeadingFont = new("Segoe UI", 16f, FontStyle.Bold);
        public static readonly Font SubheadingFont = new("Segoe UI", 11f, FontStyle.Bold);

        public static void StyleForm(Form form)
        {
            form.Font = BaseFont;
            form.BackColor = Background;
        }

        /// <summary>Same idea as StyleForm but for a UserControl page hosted inside the dashboard's content area.</summary>
        public static void StyleControl(Control control)
        {
            control.Font = BaseFont;
            control.BackColor = Background;
        }

        public static void StylePrimaryButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = PrimaryDark;
            button.BackColor = Primary;
            button.ForeColor = Color.White;
            button.Font = BoldFont;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        public static void StyleDangerButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = DangerDark;
            button.BackColor = Danger;
            button.ForeColor = Color.White;
            button.Font = BoldFont;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        public static void StyleSecondaryButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 246, 248);
            button.BackColor = Color.White;
            button.ForeColor = TextPrimary;
            button.Font = BaseFont;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        public static void StyleTextBox(TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = BaseFont;
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = CardBackground;
            grid.GridColor = Color.FromArgb(233, 235, 239);
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(247, 248, 250);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.Font = BoldFont;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
            grid.ColumnHeadersHeight = 38;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.RowHeadersVisible = false;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.RowTemplate.Height = 34;
            grid.DefaultCellStyle.Font = BaseFont;
            grid.DefaultCellStyle.SelectionBackColor = Primary;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.DefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 252);
            grid.AllowUserToResizeRows = false;
        }

        /// <summary>Flat "card" panel with a subtle border - used to lift content off the plain background.</summary>
        public static Panel CreateCard()
        {
            var panel = new Panel { BackColor = CardBackground, Padding = new Padding(16) };
            panel.Paint += (_, e) =>
            {
                using var pen = new Pen(Border);
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            };
            return panel;
        }

        /// <summary>A small rounded status pill (e.g. "In Stock", "Low Stock", "Inactive") drawn with GDI+ so no image assets are needed.</summary>
        public static Label CreateBadge(string text, Color background, Color foreground)
        {
            var badge = new Label
            {
                Text = text,
                AutoSize = true,
                Font = SmallBoldFont,
                ForeColor = foreground,
                BackColor = background,
                Padding = new Padding(8, 3, 8, 3),
                Margin = new Padding(0)
            };
            badge.Paint += (_, e) =>
            {
                using var brush = new SolidBrush(background);
                using var path = RoundedRect(new Rectangle(0, 0, badge.Width - 1, badge.Height - 1), 8);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillPath(brush, path);
            };
            return badge;
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Loads a product image from disk for display in a PictureBox, returning a
        /// generic "no image" placeholder (drawn with GDI+, no external asset needed)
        /// when the product has no image or the file can no longer be found.
        /// </summary>
        public static Image LoadProductImageOrPlaceholder(string? relativeImagePath, int size = 96)
        {
            if (!string.IsNullOrWhiteSpace(relativeImagePath))
            {
                string fullPath = Path.Combine(AppContext.BaseDirectory, relativeImagePath);
                if (File.Exists(fullPath))
                {
                    try
                    {
                        using var stream = new MemoryStream(File.ReadAllBytes(fullPath));
                        return Image.FromStream(stream);
                    }
                    catch
                    {
                        // Falls through to the placeholder below if the file is missing/corrupt.
                    }
                }
            }

            return CreatePlaceholderImage(size);
        }

        private static Image CreatePlaceholderImage(int size)
        {
            var bitmap = new Bitmap(size, size);
            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(243, 244, 246));
            using var borderPen = new Pen(Border, 1);
            g.DrawRectangle(borderPen, 0, 0, size - 1, size - 1);

            // Simple picture-frame glyph so an empty product card doesn't look broken.
            using var iconPen = new Pen(Color.FromArgb(196, 200, 209), Math.Max(1.5f, size / 40f));
            int margin = size / 4;
            g.DrawRectangle(iconPen, margin, margin, size - margin * 2, size - margin * 2);
            var mountainPoints = new[]
            {
                new Point(margin + 2, size - margin - 4),
                new Point(size / 2 - size / 10, size / 2 + size / 12),
                new Point(size / 2 + size / 14, size - margin - size / 6),
                new Point(size - margin - 4, size / 2 + size / 10),
                new Point(size - margin - 4, size - margin - 4)
            };
            g.DrawLines(iconPen, mountainPoints);
            int sunD = Math.Max(4, size / 8);
            g.DrawEllipse(iconPen, size - margin - sunD - size / 10, margin + size / 12, sunD, sunD);

            return bitmap;
        }
    }
}
