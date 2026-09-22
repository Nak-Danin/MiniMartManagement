using MiniMartManagement.Models;
using MiniMartManagement.Services;
using MiniMartManagement.Services.Exceptions;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>Embedded (not a popup) category-management page - hosted directly in DashboardForm's content area.</summary>
    public class CategoryManagementPanel : UserControl
    {
        private readonly AppServices _services;
        private readonly User _currentUser;
        private readonly DataGridView _grid = new();

        public CategoryManagementPanel(AppServices services, User currentUser)
        {
            _services = services;
            _currentUser = currentUser;
            Dock = DockStyle.Fill;
            BuildUi();
            LoadCategories();
        }

        private void BuildUi()
        {
            UiTheme.StyleControl(this);

            _grid.Dock = DockStyle.Fill;
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = false;
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Category Name", Width = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", Width = 260 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "IsActive", HeaderText = "Active", Width = 70 });
            UiTheme.StyleGrid(_grid);

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 54, Padding = new Padding(10), BackColor = UiTheme.CardBackground };
            var addButton = new Button { Text = "Add Category", Location = new Point(10, 11), Size = new Size(120, 30) };
            UiTheme.StylePrimaryButton(addButton);
            var editButton = new Button { Text = "Edit", Location = new Point(140, 11), Size = new Size(90, 30) };
            UiTheme.StyleSecondaryButton(editButton);
            var toggleButton = new Button { Text = "Activate/Deactivate", Location = new Point(240, 11), Size = new Size(150, 30) };
            UiTheme.StyleSecondaryButton(toggleButton);

            addButton.Click += AddButton_Click;
            editButton.Click += EditButton_Click;
            toggleButton.Click += ToggleButton_Click;

            bottomPanel.Controls.AddRange(new Control[] { addButton, editButton, toggleButton });

            Controls.Add(_grid);
            Controls.Add(bottomPanel);
        }

        private void LoadCategories()
        {
            _grid.Rows.Clear();
            foreach (Category category in _services.Categories.GetAll())
            {
                int rowIndex = _grid.Rows.Add(category.Name, category.Description, category.IsActive ? "Yes" : "No");
                _grid.Rows[rowIndex].Tag = category;
            }
        }

        private Category? GetSelectedCategory()
        {
            if (_grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a category first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            return (Category)_grid.SelectedRows[0].Tag!;
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new CategoryEditDialog();
            if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;

            try
            {
                _services.Categories.AddCategory(_currentUser, dialog.CategoryName, dialog.Description);
                LoadCategories();
            }
            catch (Exception ex) when (ex is BusinessRuleException or ArgumentException or UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message, "Could Not Add Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void EditButton_Click(object? sender, EventArgs e)
        {
            Category? category = GetSelectedCategory();
            if (category == null) return;

            using var dialog = new CategoryEditDialog(category);
            if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;

            try
            {
                _services.Categories.UpdateCategory(_currentUser, category, dialog.CategoryName, dialog.Description);
                LoadCategories();
            }
            catch (Exception ex) when (ex is BusinessRuleException or ArgumentException or UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message, "Could Not Update Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ToggleButton_Click(object? sender, EventArgs e)
        {
            Category? category = GetSelectedCategory();
            if (category == null) return;

            bool newActive = !category.IsActive;
            try
            {
                _services.Categories.SetCategoryActive(_currentUser, category.Id, newActive);
                LoadCategories();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
