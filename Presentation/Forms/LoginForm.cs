using MiniMartManagement.Models;
using MiniMartManagement.Services;
using MiniMartManagement.Services.Exceptions;

namespace MiniMartManagement.Presentation.Forms
{
    public partial class LoginForm : Form
    {
        private readonly AppServices? _services;
        public User? LoggedInUser { get; private set; }

        // Parameterless ctor required so the WinForms designer can instantiate the form.
        public LoginForm()
        {
            InitializeComponent();

            // Apply runtime styles that depend on UiTheme (designer won't run these).
            BackColor = UiTheme.Background;
            // Apply textbox/button styles (safe to call in designer too).
            UiTheme.StyleTextBox(_usernameBox);
            UiTheme.StyleTextBox(_passwordBox);
            UiTheme.StylePrimaryButton(_loginButton);
            AcceptButton = _loginButton;
        }

        // Runtime constructor used by Program.cs
        public LoginForm(AppServices services) : this()
        {
            _services = services;
        }

        private static void CenterCard(Panel card, Panel container)
        {
            card.Location = new Point(
                Math.Max(0, (container.Width - card.Width) / 2),
                Math.Max(0, (container.Height - card.Height) / 2));
        }

        private void RightPanel_Resize(object? sender, EventArgs e)
        {
            CenterCard(_cardPanel, _rightPanel);
        }

        private void LoginButton_Click(object? sender, EventArgs e)
        {
            _errorLabel.Text = string.Empty;

            try
            {
                // _services is provided at runtime by Program.cs; suppress nullable warning with !
                LoggedInUser = _services!.Auth.Login(_usernameBox.Text.Trim(), _passwordBox.Text);
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
