using MiniMartManagement.Models;
using MiniMartManagement.Services;
using MiniMartManagement.Services.Exceptions;

namespace MiniMartManagement.Presentation.Forms
{
    /// <summary>Embedded (not a popup) employee-management page - hosted directly in DashboardForm's content area.</summary>
    public partial class EmployeeManagementPanel : UserControl
    {
        private readonly AppServices _services;
        private readonly User _currentUser;

        private readonly DataGridView _grid = new();
        private readonly TextBox _searchBox = new();

        public EmployeeManagementPanel(AppServices services, User currentUser)
        {
            _services = services;
            _currentUser = currentUser;
            Dock = DockStyle.Fill;
            BuildUi();
            LoadEmployees();
        }

        private void BuildUi()
        {
            UiTheme.StyleControl(this);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(10, 8, 10, 8) };
            _searchBox.Location = new Point(10, 10);
            _searchBox.Size = new Size(250, 25);
            
            var searchButton = new Button { Text = "Search", Location = new Point(270, 8), Size = new Size(80, 27) };
            UiTheme.StylePrimaryButton(searchButton);
            var clearButton = new Button { Text = "Clear", Location = new Point(355, 8), Size = new Size(80, 27) };
            UiTheme.StyleSecondaryButton(clearButton);
            searchButton.Click += (_, _) => LoadEmployees(_searchBox.Text.Trim());
            clearButton.Click += (_, _) => { _searchBox.Clear(); LoadEmployees(); };
            topPanel.Controls.AddRange(new Control[] { _searchBox, searchButton, clearButton });

            _grid.Dock = DockStyle.Fill;
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = false;
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Username", Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "FullName", HeaderText = "Full Name", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Phone", Width = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", Width = 160 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "HireDate", HeaderText = "Hire Date", Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 80 });
            UiTheme.StyleGrid(_grid);

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 54, Padding = new Padding(10), BackColor = UiTheme.CardBackground };
            var addButton = new Button { Text = "Add Employee", Location = new Point(10, 11), Size = new Size(120, 30) };
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
            Controls.Add(topPanel);
        }

        private void LoadEmployees(string? keyword = null)
        {
            List<Employee> employees = string.IsNullOrWhiteSpace(keyword)
                ? _services.Employees.GetAll(_currentUser)
                : _services.Employees.Search(_currentUser, keyword);

            _grid.Rows.Clear();
            foreach (Employee employee in employees)
            {
                int rowIndex = _grid.Rows.Add(
                    employee.Username, employee.FullName, employee.Phone, employee.Email,
                    employee.HireDate.ToString("yyyy-MM-dd"), employee.Status);
                _grid.Rows[rowIndex].Tag = employee;
            }
        }

        private Employee? GetSelectedEmployee()
        {
            if (_grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select an employee first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            return (Employee)_grid.SelectedRows[0].Tag!;
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new EmployeeEditDialog(isAddMode: true);
            if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;

            try
            {
                _services.Employees.AddEmployee(
                    _currentUser, dialog.Username, dialog.Password,
                    dialog.FirstName, dialog.LastName, dialog.Phone, dialog.Email, dialog.Address, dialog.HireDate);
                LoadEmployees();
            }
            catch (Exception ex) when (ex is BusinessRuleException or ArgumentException or UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message, "Could Not Add Employee", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void EditButton_Click(object? sender, EventArgs e)
        {
            Employee? employee = GetSelectedEmployee();
            if (employee == null) return;

            using var dialog = new EmployeeEditDialog(isAddMode: false, employee);
            if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;

            try
            {
                _services.Employees.UpdateEmployee(
                    _currentUser, employee, dialog.FirstName, dialog.LastName, dialog.Phone, dialog.Email, dialog.Address);
                LoadEmployees();
            }
            catch (Exception ex) when (ex is BusinessRuleException or ArgumentException or UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message, "Could Not Update Employee", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ToggleButton_Click(object? sender, EventArgs e)
        {
            Employee? employee = GetSelectedEmployee();
            if (employee == null) return;

            EmployeeStatus newStatus = employee.Status == EmployeeStatus.Active
                ? EmployeeStatus.Inactive
                : EmployeeStatus.Active;

            var confirm = MessageBox.Show(
                $"Set {employee.FullName}'s status to {newStatus}?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _services.Employees.SetEmployeeStatus(_currentUser, employee.EmployeeId, newStatus);
                LoadEmployees();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
