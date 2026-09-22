namespace MiniMartManagement.Models
{
    /// <summary>
    /// An Admin is just a User with elevated permissions - no extra
    /// data fields needed, so this class stays small on purpose.
    /// </summary>
    public sealed class Admin : User
    {
        public Admin(string username, string passwordHash, bool isActive, DateTime createdAt)
            : base(username, passwordHash, isActive, createdAt)
        {
        }

        public override UserRole Role => UserRole.Admin;

        public override string GetDashboardTitle() => "Admin Dashboard";

        public override bool CanManageEmployees() => true;
        public override bool CanManageProductsAndCategories() => true;
        public override bool CanManageInventory() => true;
        public override bool CanViewAllSalesReports() => true;
    }
}
