using MiniMartManagement.Models;
using MiniMartManagement.Services;
using MiniMartManagement.Services.Exceptions;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>
    /// Shown modally from Program.cs. Sets DialogResult.OK and LoggedInUser
    /// on a successful login; closing the window any other way (X button,
    /// no login attempted) leaves DialogResult at its default, which
    /// Program.cs treats as "exit the application".
    /// </summary>
    public class LoginForm : Form
    {
        private readonly AppServices _services;

        private readonly TextBox _usernameBox = new();
        private readonly TextBox _passwordBox = new();
        private readonly Label _errorLabel = new();

        public User? LoggedInUser { get; private set; }

        public LoginForm(AppServices services)
        {
            _services = services;
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "MiniMart Management System - Login";
            ClientSize = new Size(980, 600);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = UiTheme.Background;

            // Left brand panel - purely visual, gives the login screen some personality.
            var brandPanel = new Panel { Dock = DockStyle.Left, Width = 460, BackColor = UiTheme.SidebarBackground };
            var brandTitle = new Label
            {
                Text = "MiniMart",
                Font = new Font("Segoe UI", 30, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Location = new Point(50, 230),
                Size = new Size(380, 60)
            };
            var brandSubtitle = new Label
            {
                Text = "Management System",
                Font = new Font("Segoe UI", 13),
                ForeColor = UiTheme.SidebarSubtleText,
                AutoSize = false,
                Location = new Point(50, 295),
                Size = new Size(380, 30)
            };
            var brandAccent = new Panel { BackColor = UiTheme.Primary, Location = new Point(50, 210), Size = new Size(56, 6) };
            brandPanel.Controls.AddRange(new Control[] { brandAccent, brandTitle, brandSubtitle });

            // Right login card.
            var rightPanel = new Panel { Dock = DockStyle.Fill };

            var card = UiTheme.CreateCard();
            card.Size = new Size(380, 380);
            card.Location = new Point(70, 110);
            card.Anchor = AnchorStyles.None;

            var titleLabel = new Label
            {
                Text = "Welcome back",
                Font = UiTheme.HeadingFont,
                ForeColor = UiTheme.TextPrimary,
                Location = new Point(24, 20),
                AutoSize = true
            };
            var subtitleLabel = new Label
            {
                Text = "Sign in to continue",
                Font = UiTheme.BaseFont,
                ForeColor = UiTheme.TextSecondary,
                Location = new Point(24, 52),
                AutoSize = true
            };

            var usernameLabel = new Label { Text = "Username", Location = new Point(24, 100), AutoSize = true, ForeColor = UiTheme.TextSecondary };
            UiTheme.StyleTextBox(_usernameBox);
            _usernameBox.Location = new Point(24, 120);
            _usernameBox.Size = new Size(330, 28);

            var passwordLabel = new Label { Text = "Password", Location = new Point(24, 160), AutoSize = true, ForeColor = UiTheme.TextSecondary };
            UiTheme.StyleTextBox(_passwordBox);
            _passwordBox.Location = new Point(24, 180);
            _passwordBox.Size = new Size(330, 28);
            _passwordBox.UseSystemPasswordChar = true;

            _errorLabel.Location = new Point(24, 218);
            _errorLabel.Size = new Size(330, 40);
            _errorLabel.ForeColor = UiTheme.Danger;
            _errorLabel.Font = UiTheme.BaseFont;
            _errorLabel.Text = string.Empty;

            var loginButton = new Button { Text = "Login", Location = new Point(24, 270), Size = new Size(330, 40) };
            UiTheme.StylePrimaryButton(loginButton);
            loginButton.Click += LoginButton_Click;
            AcceptButton = loginButton;

            card.Controls.AddRange(new Control[]
            {
                titleLabel, subtitleLabel, usernameLabel, _usernameBox,
                passwordLabel, _passwordBox, _errorLabel, loginButton
            });

            rightPanel.Controls.Add(card);
            rightPanel.Resize += (_, _) => CenterCard(card, rightPanel);
            CenterCard(card, rightPanel);

            Controls.Add(rightPanel);
            Controls.Add(brandPanel);
        }

        private static void CenterCard(Panel card, Panel container)
        {
            card.Location = new Point(
                Math.Max(0, (container.Width - card.Width) / 2),
                Math.Max(0, (container.Height - card.Height) / 2));
        }

        private void LoginButton_Click(object? sender, EventArgs e)
        {
            _errorLabel.Text = string.Empty;

            try
            {
                LoggedInUser = _services.Auth.Login(_usernameBox.Text.Trim(), _passwordBox.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (AuthenticationException ex)
            {
                _errorLabel.Text = ex.Message;
                _passwordBox.Clear();
                _passwordBox.Focus();
            }
            catch (ArgumentException ex)
            {
                // Thrown by ValidationHelper if username/password box was left empty.
                _errorLabel.Text = ex.Message;
            }
        }
    }
}
