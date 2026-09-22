using MiniMartManagement.Models;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>
    /// One dialog handles both Add and Edit. In Add mode, username/password/
    /// hire date are shown (a new login has to be created). In Edit mode
    /// those are hidden - EmployeeService.UpdateEmployee only touches profile
    /// fields, matching what the dialog collects.
    /// </summary>
    public class EmployeeEditDialog : Form
    {
        private readonly bool _isAddMode;

        private readonly TextBox _usernameBox = new();
        private readonly TextBox _passwordBox = new();
        private readonly TextBox _firstNameBox = new();
        private readonly TextBox _lastNameBox = new();
        private readonly TextBox _phoneBox = new();
        private readonly TextBox _emailBox = new();
        private readonly TextBox _addressBox = new();
        private readonly DateTimePicker _hireDatePicker = new();
        private readonly Label _errorLabel = new();

        public string Username => _usernameBox.Text.Trim();
        public string Password => _passwordBox.Text;
        public string FirstName => _firstNameBox.Text.Trim();
        public string LastName => _lastNameBox.Text.Trim();
        public string? Phone => string.IsNullOrWhiteSpace(_phoneBox.Text) ? null : _phoneBox.Text.Trim();
        public string? Email => string.IsNullOrWhiteSpace(_emailBox.Text) ? null : _emailBox.Text.Trim();
        public string? Address => string.IsNullOrWhiteSpace(_addressBox.Text) ? null : _addressBox.Text.Trim();
        public DateOnly HireDate => DateOnly.FromDateTime(_hireDatePicker.Value);

        public EmployeeEditDialog(bool isAddMode, Employee? existing = null)
        {
            _isAddMode = isAddMode;
            BuildUi();

            if (existing != null)
            {
                _firstNameBox.Text = existing.FirstName;
                _lastNameBox.Text = existing.LastName;
                _phoneBox.Text = existing.Phone ?? string.Empty;
                _emailBox.Text = existing.Email ?? string.Empty;
                _addressBox.Text = existing.Address ?? string.Empty;
            }
        }

        private void BuildUi()
        {
            UiTheme.StyleForm(this);
            Text = _isAddMode ? "Add Employee" : "Edit Employee";

            // The previous fixed ClientSize (420 / 300) was shorter than the fields it
            // had to hold, which pushed Save/Cancel off the bottom of the dialog. This
            // version sizes itself to the actual content and scrolls if the screen is
            // too short, instead of guessing a fixed pixel height up front.
            ClientSize = new Size(380, _isAddMode ? 560 : 400);
            MinimumSize = new Size(380, 320);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScroll = true;

            var layout = new Panel { AutoSize = true, Location = new Point(0, 0), Width = 340, Padding = new Padding(15) };

            int y = 0;

            int AddField(string label, Control control, int height = 25)
            {
                layout.Controls.Add(new Label { Text = label, Location = new Point(0, y), AutoSize = true });
                y += 20;
                control.Location = new Point(0, y);
                control.Size = new Size(310, height);
                layout.Controls.Add(control);
                return y += height + 14;
            }

            if (_isAddMode)
            {
                y = AddField("Username", _usernameBox);
                _passwordBox.UseSystemPasswordChar = true;
                y = AddField("Password", _passwordBox);
            }

            y = AddField("First Name", _firstNameBox);
            y = AddField("Last Name", _lastNameBox);
            y = AddField("Phone (optional)", _phoneBox);
            y = AddField("Email (optional)", _emailBox);
            y = AddField("Address (optional)", _addressBox);

            if (_isAddMode)
            {
                _hireDatePicker.Value = DateTime.Today;
                _hireDatePicker.Format = DateTimePickerFormat.Short;
                y = AddField("Hire Date", _hireDatePicker, 25);
            }

            _errorLabel.Location = new Point(0, y);
            _errorLabel.Size = new Size(310, 40);
            _errorLabel.ForeColor = Color.Firebrick;
            layout.Controls.Add(_errorLabel);
            y += 45;

            var okButton = new Button { Text = "Save", Location = new Point(0, y), Size = new Size(150, 34) };
            UiTheme.StylePrimaryButton(okButton);
            var cancelButton = new Button { Text = "Cancel", Location = new Point(160, y), Size = new Size(150, 34) };
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

        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                _errorLabel.Text = "First name and last name are required.";
                return;
            }

            if (_isAddMode && (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)))
            {
                _errorLabel.Text = "Username and password are required.";
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
