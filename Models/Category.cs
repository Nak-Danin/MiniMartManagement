using MiniMartManagement.Utilities;

namespace MiniMartManagement.Models
{
    public class Category
    {
        public int Id { get; internal set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        public Category(string name, string? description, bool isActive)
        {
            ValidationHelper.EnsureNotEmpty(name, nameof(name));

            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public void UpdateDetails(string name, string? description)
        {
            ValidationHelper.EnsureNotEmpty(name, nameof(name));

            Name = name;
            Description = description;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}
