namespace MiniMartManagement.Services.Exceptions
{
    /// <summary>
    /// Thrown when an action violates a business rule (duplicate username,
    /// insufficient stock, discount too large, etc.). The message is meant
    /// to be shown directly to the user - it's written for them, not for a log file.
    /// </summary>
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message)
        {
        }
    }
}
