namespace MiniMartManagement.Services.Exceptions
{
    /// <summary>Thrown for login/credential failures - Forms catch this to show a login-specific error.</summary>
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message)
        {
        }
    }
}
