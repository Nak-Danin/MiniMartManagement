using MiniMartManagement.Models;
using MiniMartManagement.Utilities;

namespace MiniMartManagement.Presentation.Forms
{
    public partial class ProductEditDialog : Form
    {
        private readonly bool _isEdit;
        private string? _imagePath;

        public int SelectedCategoryId => ((Category)_categoryCombo.SelectedItem!).Id;
        public string ProductCode => _codeBox.Text.Trim();
        public new string ProductName { get; set; }
        public decimal PurchasePrice => _purchasePriceBox.Value;
        public decimal SellingPrice => _sellingPriceBox.Value;
        public string Unit => _unitBox.Text.Trim();
        public int MinStock => (int)_minStockBox.Value;
        public string? ImagePath => _imagePath;

        // Parameterless ctor for Designer
        public ProductEditDialog()
        {
            InitializeComponent();
            UiTheme.StyleForm(this);
            UiTheme.StyleTextBox(_codeBox);
            UiTheme.StyleTextBox(_nameBox);
            UiTheme.StyleTextBox(_unitBox);
            UiTheme.StyleControl(_errorLabel);
            UiTheme.StylePrimaryButton(_okButton);
            UiTheme.StyleSecondaryButton(_cancelButton);
            UiTheme.StyleSecondaryButton(_chooseImageButton);
            UiTheme.StyleSecondaryButton(_removeImageButton);
            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }

        // Runtime ctor
        public ProductEditDialog(List<Category> categories, Product? existing = null) : this()
        {
            _isEdit = existing != null;
            _imagePath = existing?.ImagePath;

            // Populate category list
            _categoryCombo.DisplayMember = nameof(Category.Name);
            _categoryCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            _categoryCombo.DataSource = categories;

            // Configure numeric controls
            _purchasePriceBox.DecimalPlaces = 2;
            _purchasePriceBox.Maximum = 1_000_000;
            _sellingPriceBox.DecimalPlaces = 2;
            _sellingPriceBox.Maximum = 1_000_000;
            _minStockBox.Maximum = 100_000;

            // Wire events
            _chooseImageButton.Click += ChooseImageButton_Click;
            _removeImageButton.Click += (_, _) => { _imagePath = null; RefreshImagePreview(); };
            _okButton.Click += OkButton_Click;
            _cancelButton.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

            // If editing, populate fields and lock code
            if (existing != null)
            {
                _codeBox.Text = existing.ProductCode;
                _codeBox.Enabled = false;
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
