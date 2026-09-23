namespace MiniMartManagement.Presentation.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        // Controls referenced from the code-behind must be declared here.
        private System.Windows.Forms.Panel _brandPanel;
        private System.Windows.Forms.Panel _rightPanel;
        private System.Windows.Forms.Panel _cardPanel;
        private System.Windows.Forms.Label _brandTitle;
        private System.Windows.Forms.Label _brandSubtitle;
        private System.Windows.Forms.Label _titleLabel;
        private System.Windows.Forms.Label _subtitleLabel;
        private System.Windows.Forms.Label _usernameLabel;
        private System.Windows.Forms.TextBox _usernameBox;
        private System.Windows.Forms.Label _passwordLabel;
        private System.Windows.Forms.TextBox _passwordBox;
        private System.Windows.Forms.Label _errorLabel;
        private System.Windows.Forms.Button _loginButton;

        /// <summary>Required method for Designer support — do not modify the contents with the code editor.</summary>
        private void InitializeComponent()
        {
            _brandPanel = new Panel();
            _brandTitle = new Label();
            _brandSubtitle = new Label();
            _rightPanel = new Panel();
            _cardPanel = new Panel();
            _titleLabel = new Label();
            _subtitleLabel = new Label();
            _usernameLabel = new Label();
            _usernameBox = new TextBox();
            _passwordLabel = new Label();
            _passwordBox = new TextBox();
            _errorLabel = new Label();
            _loginButton = new Button();
            _brandPanel.SuspendLayout();
            _rightPanel.SuspendLayout();
            _cardPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _brandPanel
            // 
            _brandPanel.BackColor = SystemColors.Highlight;
            _brandPanel.Controls.Add(_brandTitle);
            _brandPanel.Controls.Add(_brandSubtitle);
            _brandPanel.Dock = DockStyle.Left;
            _brandPanel.Location = new Point(0, 0);
            _brandPanel.Name = "_brandPanel";
            _brandPanel.Size = new Size(460, 600);
            _brandPanel.TabIndex = 1;
            // 
            // _brandTitle
            // 
            _brandTitle.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            _brandTitle.ForeColor = Color.White;
            _brandTitle.Location = new Point(50, 230);
            _brandTitle.Name = "_brandTitle";
            _brandTitle.Size = new Size(380, 60);
            _brandTitle.TabIndex = 1;
            _brandTitle.Text = "MiniMart";
            // 
            // _brandSubtitle
            // 
            _brandSubtitle.Font = new Font("Segoe UI", 13F);
            _brandSubtitle.ForeColor = Color.FromArgb(200, 200, 200);
            _brandSubtitle.Location = new Point(50, 295);
            _brandSubtitle.Name = "_brandSubtitle";
            _brandSubtitle.Size = new Size(380, 30);
            _brandSubtitle.TabIndex = 2;
            _brandSubtitle.Text = "Management System";
            // 
            // _rightPanel
            // 
            _rightPanel.Controls.Add(_cardPanel);
            _rightPanel.Dock = DockStyle.Fill;
            _rightPanel.Location = new Point(460, 0);
            _rightPanel.Name = "_rightPanel";
            _rightPanel.Size = new Size(520, 600);
            _rightPanel.TabIndex = 0;
            _rightPanel.Resize += RightPanel_Resize;
            // 
            // _cardPanel
            // 
            _cardPanel.Anchor = AnchorStyles.None;
            _cardPanel.BackColor = Color.MediumSeaGreen;
            _cardPanel.BorderStyle = BorderStyle.FixedSingle;
            _cardPanel.Controls.Add(_titleLabel);
            _cardPanel.Controls.Add(_subtitleLabel);
            _cardPanel.Controls.Add(_usernameLabel);
            _cardPanel.Controls.Add(_usernameBox);
            _cardPanel.Controls.Add(_passwordLabel);
            _cardPanel.Controls.Add(_passwordBox);
            _cardPanel.Controls.Add(_errorLabel);
            _cardPanel.Controls.Add(_loginButton);
            _cardPanel.Location = new Point(67, 89);
            _cardPanel.Name = "_cardPanel";
            _cardPanel.Size = new Size(384, 411);
            _cardPanel.TabIndex = 0;
            // 
            // _titleLabel
            // 
            _titleLabel.AutoSize = true;
            _titleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            _titleLabel.Location = new Point(24, 20);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Size = new Size(149, 28);
            _titleLabel.TabIndex = 0;
            _titleLabel.Text = "Welcome back";
            // 
            // _subtitleLabel
            // 
            _subtitleLabel.AutoSize = true;
            _subtitleLabel.Font = new Font("Segoe UI", 9F);
            _subtitleLabel.Location = new Point(24, 52);
            _subtitleLabel.Name = "_subtitleLabel";
            _subtitleLabel.Size = new Size(133, 20);
            _subtitleLabel.TabIndex = 1;
            _subtitleLabel.Text = "Sign in to continue";
            // 
            // _usernameLabel
            // 
            _usernameLabel.AutoSize = true;
            _usernameLabel.Location = new Point(24, 100);
            _usernameLabel.Name = "_usernameLabel";
            _usernameLabel.Size = new Size(75, 20);
            _usernameLabel.TabIndex = 2;
            _usernameLabel.Text = "Username";
            // 
            // _usernameBox
            // 
            _usernameBox.Location = new Point(24, 120);
            _usernameBox.Name = "_usernameBox";
            _usernameBox.Size = new Size(330, 27);
            _usernameBox.TabIndex = 3;
            // 
            // _passwordLabel
            // 
            _passwordLabel.AutoSize = true;
            _passwordLabel.Location = new Point(24, 160);
            _passwordLabel.Name = "_passwordLabel";
            _passwordLabel.Size = new Size(70, 20);
            _passwordLabel.TabIndex = 4;
            _passwordLabel.Text = "Password";
            // 
            // _passwordBox
            // 
            _passwordBox.Location = new Point(24, 180);
            _passwordBox.Name = "_passwordBox";
            _passwordBox.Size = new Size(330, 27);
            _passwordBox.TabIndex = 5;
            _passwordBox.UseSystemPasswordChar = true;
            // 
            // _errorLabel
            // 
            _errorLabel.ForeColor = Color.Red;
            _errorLabel.Location = new Point(24, 218);
            _errorLabel.Name = "_errorLabel";
            _errorLabel.Size = new Size(330, 40);
            _errorLabel.TabIndex = 6;
            // 
            // _loginButton
            // 
            _loginButton.BackColor = Color.DodgerBlue;
            _loginButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _loginButton.ForeColor = SystemColors.Control;
            _loginButton.Location = new Point(24, 270);
            _loginButton.Name = "_loginButton";
            _loginButton.Size = new Size(330, 40);
            _loginButton.TabIndex = 7;
            _loginButton.Text = "Login";
            _loginButton.UseVisualStyleBackColor = false;
            _loginButton.Click += LoginButton_Click;
            // 
            // LoginForm
            // 
            ClientSize = new Size(980, 600);
            Controls.Add(_rightPanel);
            Controls.Add(_brandPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MiniMart Management System - Login";
            _brandPanel.ResumeLayout(false);
            _rightPanel.ResumeLayout(false);
            _cardPanel.ResumeLayout(false);
            _cardPanel.PerformLayout();
            ResumeLayout(false);
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