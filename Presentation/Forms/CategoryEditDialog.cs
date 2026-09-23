using MiniMartManagement.Models;

namespace MiniMartManagement.Presentation.Forms
{
    public partial class CategoryEditDialog : Form
    {
        public string CategoryName => _nameBox.Text.Trim();
        public string? Description => string.IsNullOrWhiteSpace(_descriptionBox.Text) ? null : _descriptionBox.Text.Trim();

        // Parameterless ctor for the WinForms Designer.
        public CategoryEditDialog()
        {
            InitializeComponent();

            // Runtime styling and behavior (safe for designer).
            UiTheme.StyleForm(this);
            UiTheme.StylePrimaryButton(_okButton);
            UiTheme.StyleSecondaryButton(_cancelButton);
            UiTheme.StyleTextBox(_nameBox);
            UiTheme.StyleTextBox(_descriptionBox);

            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }

        // Runtime ctor used by code to open the dialog with an existing category.
        public CategoryEditDialog(Category? existing = null) : this()
        {
            if (existing != null)
            {
                _nameBox.Text = existing.Name;
                _descriptionBox.Text = existing.Description ?? string.Empty;
            }
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            _errorLabel.Text = string.Empty;
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                _errorLabel.Text = "Category name is required.";
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
