using MiniMartManagement.Models;

namespace MiniMartManagement.Presentation.Forms
{
    public partial class EmployeeEditDialog : Form
    {
        private readonly bool _isAddMode;

        public string Username => _usernameBox.Text.Trim();
        public string Password => _passwordBox.Text;
        public string FirstName => _firstNameBox.Text.Trim();
        public string LastName => _lastNameBox.Text.Trim();
        public string? Phone => string.IsNullOrWhiteSpace(_phoneBox.Text) ? null : _phoneBox.Text.Trim();
        public string? Email => string.IsNullOrWhiteSpace(_emailBox.Text) ? null : _emailBox.Text.Trim();
        public string? Address => string.IsNullOrWhiteSpace(_addressBox.Text) ? null : _addressBox.Text.Trim();
        public DateOnly HireDate => DateOnly.FromDateTime(_hireDatePicker.Value);

        // Parameterless ctor for the Designer.
        public EmployeeEditDialog()
        {
            InitializeComponent();

            UiTheme.StyleForm(this);
            UiTheme.StyleTextBox(_usernameBox);
            UiTheme.StyleTextBox(_passwordBox);
            UiTheme.StyleTextBox(_firstNameBox);
            UiTheme.StyleTextBox(_lastNameBox);
            UiTheme.StyleTextBox(_phoneBox);
            UiTheme.StyleTextBox(_emailBox);
            UiTheme.StyleTextBox(_addressBox);

            UiTheme.StylePrimaryButton(_okButton);
            UiTheme.StyleSecondaryButton(_cancelButton);

            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }

        // Runtime ctor used by callers.
        public EmployeeEditDialog(bool isAddMode, Employee? existing = null) : this()
        {
            _isAddMode = isAddMode;
            Text = _isAddMode ? "Add Employee" : "Edit Employee";

            // Show/hide username/password/hire date controls for Add vs Edit
            _usernameLabel.Visible = _usernameBox.Visible = _passwordLabel.Visible = _passwordBox.Visible = _hireDateLabel.Visible = _hireDatePicker.Visible = _isAddMode;

            if (existing != null)
            {
                _firstNameBox.Text = existing.FirstName;
                _lastNameBox.Text = existing.LastName;
                _phoneBox.Text = existing.Phone ?? string.Empty;
                _emailBox.Text = existing.Email ?? string.Empty;
                _addressBox.Text = existing.Address ?? string.Empty;
                _hireDatePicker.Value = existing.HireDate.ToDateTime(new TimeOnly(0, 0));
            }
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            _errorLabel.Text = string.Empty;

            if (_isAddMode)
            {
                if (string.IsNullOrWhiteSpace(Username))
                {
                    _errorLabel.Text = "Username is required.";
                    return;
                }
                if (string.IsNullOrEmpty(Password) || Password.Length < 6)
                {
                    _errorLabel.Text = "Password must be at least 6 characters.";
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                _errorLabel.Text = "First name and last name are required.";
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
