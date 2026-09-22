using MiniMartManagement.Models;
using MiniMartManagement.Utilities;

namespace MiniMartManagement.Presentation.Forms
{
    public class ProductEditDialog : Form
    {
        private readonly ComboBox _categoryCombo = new();
        private readonly TextBox _codeBox = new();
        private readonly TextBox _nameBox = new();
        private readonly NumericUpDown _purchasePriceBox = new();
        private readonly NumericUpDown _sellingPriceBox = new();
        private readonly TextBox _unitBox = new();
        private readonly NumericUpDown _minStockBox = new();
        private readonly PictureBox _imagePreview = new();
        private readonly Label _errorLabel = new();
        private readonly bool _isEdit;

        private string? _imagePath;

        public int SelectedCategoryId => ((Category)_categoryCombo.SelectedItem!).Id;
        public string ProductCode => _codeBox.Text.Trim();
        public string ProductName => _nameBox.Text.Trim();
        public decimal PurchasePrice => _purchasePriceBox.Value;
        public decimal SellingPrice => _sellingPriceBox.Value;
        public string Unit => _unitBox.Text.Trim();
        public int MinStock => (int)_minStockBox.Value;

        /// <summary>
        /// Relative path (under the app folder) of the product's photo, or null if none
        /// was chosen. Always reflects the current state of the dialog - unchanged,
        /// newly picked, or cleared - so the caller can pass it straight through.
        /// </summary>
        public string? ImagePath => _imagePath;

        public ProductEditDialog(List<Category> categories, Product? existing = null)
        {
            _isEdit = existing != null;
            _imagePath = existing?.ImagePath;
            BuildUi(categories);

            if (existing != null)
            {
                _codeBox.Text = existing.ProductCode;
                _codeBox.Enabled = false; // product code is immutable once created (matches ProductService, which never updates it)
                _nameBox.Text = existing.Name;
                _purchasePriceBox.Value = existing.PurchasePrice;
                _sellingPriceBox.Value = existing.SellingPrice;
                _unitBox.Text = existing.Unit;
                _minStockBox.Value = existing.MinStock;

                foreach (Category category in categories)
                {
                    if (category.Id == existing.CategoryId)
                    {
                        _categoryCombo.SelectedItem = category;
                        break;
                    }
                }
            }

            RefreshImagePreview();
        }

        private void BuildUi(List<Category> categories)
        {
            UiTheme.StyleForm(this);
            Text = _isEdit ? "Edit Product" : "Add Product";
            // Sized generously and scrollable, rather than a tight fixed height that
            // clips the Save/Cancel buttons once every field (including the image
            // picker) is stacked in - that was the root cause of the "half-hidden"
            // dialogs reported in Step 8.
            ClientSize = new Size(420, 560);
            MinimumSize = new Size(420, 420);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScroll = true;

            var layout = new Panel { AutoSize = true, Location = new Point(0, 0), Width = 380, Padding = new Padding(15, 15, 15, 15) };

            int y = 0;

            // ---- Photo picker ----
            layout.Controls.Add(new Label { Text = "Product Photo", Location = new Point(0, y), AutoSize = true });
            y += 20;
            _imagePreview.Location = new Point(0, y);
            _imagePreview.Size = new Size(96, 96);
            _imagePreview.SizeMode = PictureBoxSizeMode.Zoom;
            _imagePreview.BorderStyle = BorderStyle.FixedSingle;
            layout.Controls.Add(_imagePreview);

            var chooseImageButton = new Button { Text = "Choose Image...", Location = new Point(108, y), Size = new Size(130, 30) };
            UiTheme.StyleSecondaryButton(chooseImageButton);
            chooseImageButton.Click += ChooseImageButton_Click;

            var removeImageButton = new Button { Text = "Remove", Location = new Point(244, y), Size = new Size(106, 30) };
            UiTheme.StyleSecondaryButton(removeImageButton);
            removeImageButton.Click += (_, _) => { _imagePath = null; RefreshImagePreview(); };

            layout.Controls.Add(chooseImageButton);
            layout.Controls.Add(removeImageButton);
            y += 96 + 16;

            // ---- Fields ----
            _categoryCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            _categoryCombo.DisplayMember = nameof(Category.Name);
            _categoryCombo.DataSource = categories;
            y = AddFieldAt(layout, "Category", _categoryCombo, y);

            y = AddFieldAt(layout, "Product Code", _codeBox, y);
            y = AddFieldAt(layout, "Product Name", _nameBox, y);

            _purchasePriceBox.DecimalPlaces = 2;
            _purchasePriceBox.Maximum = 1_000_000;
            y = AddFieldAt(layout, "Purchase Price", _purchasePriceBox, y);

            _sellingPriceBox.DecimalPlaces = 2;
            _sellingPriceBox.Maximum = 1_000_000;
            y = AddFieldAt(layout, "Selling Price", _sellingPriceBox, y);

            y = AddFieldAt(layout, "Unit (e.g. bottle, pack)", _unitBox, y);

            _minStockBox.Maximum = 100_000;
            y = AddFieldAt(layout, "Minimum Stock Level", _minStockBox, y);

            _errorLabel.Location = new Point(0, y);
            _errorLabel.Size = new Size(350, 35);
            _errorLabel.ForeColor = Color.Firebrick;
            layout.Controls.Add(_errorLabel);
            y += 40;

            var okButton = new Button { Text = "Save", Location = new Point(0, y), Size = new Size(170, 34) };
            UiTheme.StylePrimaryButton(okButton);
            var cancelButton = new Button { Text = "Cancel", Location = new Point(180, y), Size = new Size(170, 34) };
            UiTheme.StyleSecondaryButton(cancelButton);
            okButton.Click += OkButton_Click;
            cancelButton.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

            layout.Controls.Add(okButton);
            layout.Controls.Add(cancelButton);
            y += 34 + 15;

            layout.Height = y;

            AcceptButton = okButton;
            CancelButton = cancelButton;
            Controls.Add(layout);
        }

        private static int AddFieldAt(Panel layout, string label, Control control, int y, int height = 25)
        {
            layout.Controls.Add(new Label { Text = label, Location = new Point(0, y), AutoSize = true });
            y += 20;
            control.Location = new Point(0, y);
            control.Size = new Size(350, height);
            layout.Controls.Add(control);
            return y + height + 14;
        }

        private void ChooseImageButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Choose a product photo",
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                _imagePath = ProductImageStore.SaveCopy(dialog.FileName);
                RefreshImagePreview();
            }
            catch (Exception ex)
            {
                _errorLabel.Text = $"Could not use that image: {ex.Message}";
            }
        }

        private void RefreshImagePreview()
        {
            _imagePreview.Image?.Dispose();
            _imagePreview.Image = UiTheme.LoadProductImageOrPlaceholder(_imagePath, 96);
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (_categoryCombo.SelectedItem == null)
            {
                _errorLabel.Text = "Please select a category.";
                return;
            }

            if (string.IsNullOrWhiteSpace(ProductCode) || string.IsNullOrWhiteSpace(ProductName) || string.IsNullOrWhiteSpace(Unit))
            {
                _errorLabel.Text = "Product code, name, and unit are required.";
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
