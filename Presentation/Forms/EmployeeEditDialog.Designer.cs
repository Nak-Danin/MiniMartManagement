namespace MiniMartManagement.Presentation.Forms
{
    partial class EmployeeEditDialog
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label _usernameLabel;
        private System.Windows.Forms.TextBox _usernameBox;
        private System.Windows.Forms.Label _passwordLabel;
        private System.Windows.Forms.TextBox _passwordBox;
        private System.Windows.Forms.Label _firstNameLabel;
        private System.Windows.Forms.TextBox _firstNameBox;
        private System.Windows.Forms.Label _lastNameLabel;
        private System.Windows.Forms.TextBox _lastNameBox;
        private System.Windows.Forms.Label _phoneLabel;
        private System.Windows.Forms.TextBox _phoneBox;
        private System.Windows.Forms.Label _emailLabel;
        private System.Windows.Forms.TextBox _emailBox;
        private System.Windows.Forms.Label _addressLabel;
        private System.Windows.Forms.TextBox _addressBox;
        private System.Windows.Forms.Label _hireDateLabel;
        private System.Windows.Forms.DateTimePicker _hireDatePicker;
        private System.Windows.Forms.Label _errorLabel;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;

        /// <summary>Required method for Designer support — do not modify the contents with the code editor.</summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this._usernameLabel = new System.Windows.Forms.Label();
            this._usernameBox = new System.Windows.Forms.TextBox();
            this._passwordLabel = new System.Windows.Forms.Label();
            this._passwordBox = new System.Windows.Forms.TextBox();
            this._firstNameLabel = new System.Windows.Forms.Label();
            this._firstNameBox = new System.Windows.Forms.TextBox();
            this._lastNameLabel = new System.Windows.Forms.Label();
            this._lastNameBox = new System.Windows.Forms.TextBox();
            this._phoneLabel = new System.Windows.Forms.Label();
            this._phoneBox = new System.Windows.Forms.TextBox();
            this._emailLabel = new System.Windows.Forms.Label();
            this._emailBox = new System.Windows.Forms.TextBox();
            this._addressLabel = new System.Windows.Forms.Label();
            this._addressBox = new System.Windows.Forms.TextBox();
            this._hireDateLabel = new System.Windows.Forms.Label();
            this._hireDatePicker = new System.Windows.Forms.DateTimePicker();
            this._errorLabel = new System.Windows.Forms.Label();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();

            // 
            // EmployeeEditDialog (this)
            // 
            this.ClientSize = new System.Drawing.Size(420, 520);
            this.MinimumSize = new System.Drawing.Size(380, 320);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.AutoScroll = true;

            int left = 18;
            int inputWidth = 360;
            int y = 12;

            // Username
            this._usernameLabel.Text = "Username";
            this._usernameLabel.Location = new System.Drawing.Point(left, y);
            this._usernameLabel.AutoSize = true;
            y += 22;
            this._usernameBox.Location = new System.Drawing.Point(left, y);
            this._usernameBox.Size = new System.Drawing.Size(inputWidth, 25);
            y += 38;

            // Password
            this._passwordLabel.Text = "Password";
            this._passwordLabel.Location = new System.Drawing.Point(left, y);
            this._passwordLabel.AutoSize = true;
            y += 22;
            this._passwordBox.Location = new System.Drawing.Point(left, y);
            this._passwordBox.Size = new System.Drawing.Size(inputWidth, 25);
            this._passwordBox.UseSystemPasswordChar = true;
            y += 38;

            // First name
            this._firstNameLabel.Text = "First name";
            this._firstNameLabel.Location = new System.Drawing.Point(left, y);
            this._firstNameLabel.AutoSize = true;
            y += 22;
            this._firstNameBox.Location = new System.Drawing.Point(left, y);
            this._firstNameBox.Size = new System.Drawing.Size(inputWidth, 25);
            y += 38;

            // Last name
            this._lastNameLabel.Text = "Last name";
            this._lastNameLabel.Location = new System.Drawing.Point(left, y);
            this._lastNameLabel.AutoSize = true;
            y += 22;
            this._lastNameBox.Location = new System.Drawing.Point(left, y);
            this._lastNameBox.Size = new System.Drawing.Size(inputWidth, 25);
            y += 38;

            // Phone
            this._phoneLabel.Text = "Phone (optional)";
            this._phoneLabel.Location = new System.Drawing.Point(left, y);
            this._phoneLabel.AutoSize = true;
            y += 22;
            this._phoneBox.Location = new System.Drawing.Point(left, y);
            this._phoneBox.Size = new System.Drawing.Size(inputWidth, 25);
            y += 38;

            // Email
            this._emailLabel.Text = "Email (optional)";
            this._emailLabel.Location = new System.Drawing.Point(left, y);
            this._emailLabel.AutoSize = true;
            y += 22;
            this._emailBox.Location = new System.Drawing.Point(left, y);
            this._emailBox.Size = new System.Drawing.Size(inputWidth, 25);
            y += 38;

            // Address
            this._addressLabel.Text = "Address (optional)";
            this._addressLabel.Location = new System.Drawing.Point(left, y);
            this._addressLabel.AutoSize = true;
            y += 22;
            this._addressBox.Location = new System.Drawing.Point(left, y);
            this._addressBox.Size = new System.Drawing.Size(inputWidth, 60);
            this._addressBox.Multiline = true;
            y += 74;

            // Hire date
            this._hireDateLabel.Text = "Hire date";
            this._hireDateLabel.Location = new System.Drawing.Point(left, y);
            this._hireDateLabel.AutoSize = true;
            y += 22;
            this._hireDatePicker.Location = new System.Drawing.Point(left, y);
            this._hireDatePicker.Size = new System.Drawing.Size(inputWidth, 26);
            y += 40;

            // Error label
            this._errorLabel.Location = new System.Drawing.Point(left, y);
            this._errorLabel.Size = new System.Drawing.Size(inputWidth, 28);
            this._errorLabel.ForeColor = System.Drawing.Color.Firebrick;
            this._errorLabel.Text = string.Empty;
            y += 36;

            // Buttons
            this._okButton.Text = "Save";
            this._okButton.Location = new System.Drawing.Point(left, y);
            this._okButton.Size = new System.Drawing.Size(160, 32);
            this._okButton.Click += new System.EventHandler(this.OkButton_Click);

            this._cancelButton.Text = "Cancel";
            this._cancelButton.Location = new System.Drawing.Point(left + 180, y);
            this._cancelButton.Size = new System.Drawing.Size(160, 32);
            this._cancelButton.Click += new System.EventHandler(this.CancelButton_Click);

            // Add controls
            this.Controls.Add(this._usernameLabel);
            this.Controls.Add(this._usernameBox);
            this.Controls.Add(this._passwordLabel);
            this.Controls.Add(this._passwordBox);
            this.Controls.Add(this._firstNameLabel);
            this.Controls.Add(this._firstNameBox);
            this.Controls.Add(this._lastNameLabel);
            this.Controls.Add(this._lastNameBox);
            this.Controls.Add(this._phoneLabel);
            this.Controls.Add(this._phoneBox);
            this.Controls.Add(this._emailLabel);
            this.Controls.Add(this._emailBox);
            this.Controls.Add(this._addressLabel);
            this.Controls.Add(this._addressBox);
            this.Controls.Add(this._hireDateLabel);
            this.Controls.Add(this._hireDatePicker);
            this.Controls.Add(this._errorLabel);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
        }

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}