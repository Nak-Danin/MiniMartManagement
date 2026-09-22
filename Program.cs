using MiniMartManagement.Presentation.Forms;
using MiniMartManagement.Services;

namespace MiniMartManagement
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var appServices = new AppServices();

            // Keeps showing Login -> Dashboard -> (logout) -> Login again until
            // the person closes the login window without signing in.
            while (true)
            {
                using var loginForm = new LoginForm(appServices);
                if (loginForm.ShowDialog() != DialogResult.OK || loginForm.LoggedInUser == null)
                {
                    break;
                }

                using var dashboardForm = new DashboardForm(appServices, loginForm.LoggedInUser);
                dashboardForm.ShowDialog();

                // Always clear the session on the way back to the login screen,
                // whether the dashboard closed via Logout or the window's X button.
                appServices.Auth.Logout();
            }
        }
    }
}
