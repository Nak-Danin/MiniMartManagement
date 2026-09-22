using MiniMartManagement.Models;

namespace MiniMartManagement.Presentation.Forms
{
    public class CategoryEditDialog : Form
    {
        private readonly TextBox _nameBox = new();
        private readonly TextBox _descriptionBox = new();
        private readonly Label _errorLabel = new();

        public string CategoryName => _nameBox.Text.Trim();
        public string? Description => string.IsNullOrWhiteSpace(_descriptionBox.Text) ? null : _descriptionBox.Text.Trim();

        public CategoryEditDialog(Category? existing = null)
        {
            BuildUi(existing != null);

            if (existing != null)
            {
                _nameBox.Text = existing.Name;
                _descriptionBox.Text = existing.Description ?? string.Empty;
            }
        }

        private void BuildUi(bool isEdit)
        {
            UiTheme.StyleForm(this);
            Text = isEdit ? "Edit Category" : "Add Category";
            ClientSize = new Size(340, 260);
            MinimumSize = new Size(340, 260);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScroll = true;

            Controls.Add(new Label { Text = "Category Name", Location = new Point(15, 15), AutoSize = true });
            _nameBox.Location = new Point(15, 35);
            _nameBox.Size = new Size(300, 25);
            Controls.Add(_nameBox);

            Controls.Add(new Label { Text = "Description (optional)", Location = new Point(15, 70), AutoSize = true });
            _descriptionBox.Location = new Point(15, 90);
            _descriptionBox.Size = new Size(300, 60);
            _descriptionBox.Multiline = true;
            Controls.Add(_descriptionBox);

            _errorLabel.Location = new Point(15, 155);
            _errorLabel.Size = new Size(300, 20);
            _errorLabel.ForeColor = Color.Firebrick;
            Controls.Add(_errorLabel);

            var okButton = new Button { Text = "Save", Location = new Point(15, 200), Size = new Size(140, 32) };
            UiTheme.StylePrimaryButton(okButton);
            var cancelButton = new Button { Text = "Cancel", Location = new Point(175, 200), Size = new Size(140, 32) };
            UiTheme.StyleSecondaryButton(cancelButton);
            okButton.Click += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(CategoryName))
                {
                    _errorLabel.Text = "Category name is required.";
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            };
            cancelButton.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(okButton);
            Controls.Add(cancelButton);
            AcceptButton = okButton;
            CancelButton = cancelButton;
        }
    }
}
