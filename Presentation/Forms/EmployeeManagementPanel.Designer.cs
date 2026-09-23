using System.Drawing;
using System.Windows.Forms;

namespace MiniMartManagement.Presentation.Forms
{
    public partial class EmployeeManagementPanel
    {
        private Button _searchButton;
        private Button _clearButton;
        private Panel _topPanel;
        private Panel _bottomPanel;
        private Button _addButton;
        private Button _editButton;
        private Button _toggleButton;

        private void InitializeComponent()
        {
            UiTheme.StyleControl(this);

            // Top panel + search controls
            _topPanel = new Panel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(10, 8, 10, 8) };

            _searchBox.Location = new Point(10, 10);
            _searchBox.Size = new Size(250, 25);
            UiTheme.StyleTextBox(_searchBox);

            _searchButton = new Button { Text = "Search", Location = new Point(270, 8), Size = new Size(80, 27) };
            UiTheme.StylePrimaryButton(_searchButton);
            _searchButton.Click += (_, _) => LoadEmployees(_searchBox.Text.Trim());

            _clearButton = new Button { Text = "Clear", Location = new Point(355, 8), Size = new Size(80, 27) };
            UiTheme.StyleSecondaryButton(_clearButton);
            _clearButton.Click += (_, _) => { _searchBox.Clear(); LoadEmployees(); };

            _topPanel.Controls.AddRange(new Control[] { _searchBox, _searchButton, _clearButton });

            // Grid
            _grid.Dock = DockStyle.Fill;
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = false;
            _grid.Columns.Clear();

            // Username column: keep its selection/background consistent with other columns
            var usernameCol = new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Username", Width = 110 };
            usernameCol.HeaderCell.Style.BackColor = Color.White;
            usernameCol.HeaderCell.Style.SelectionBackColor = Color.White;
            _grid.Columns.Add(usernameCol);

            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "FullName", HeaderText = "Full Name", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Phone", Width = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", Width = 160 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "HireDate", HeaderText = "Hire Date", AutoSizeMode = DataGridViewAutoSizeColumnMode.None, Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 80 });
            UiTheme.StyleGrid(_grid);

            // Bottom panel + action buttons
            _bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 54, Padding = new Padding(10), BackColor = UiTheme.CardBackground };

            _addButton = new Button { Text = "Add Employee", Location = new Point(10, 11), Size = new Size(120, 30) };
            UiTheme.StylePrimaryButton(_addButton);
            _addButton.Click += AddButton_Click;

            _editButton = new Button { Text = "Edit", Location = new Point(140, 11), Size = new Size(90, 30) };
            UiTheme.StyleSecondaryButton(_editButton);
            _editButton.Click += EditButton_Click;

            _toggleButton = new Button { Text = "Activate/Deactivate", Location = new Point(240, 11), Size = new Size(150, 30) };
            UiTheme.StyleSecondaryButton(_toggleButton);
            _toggleButton.Click += ToggleButton_Click;

            _bottomPanel.Controls.AddRange(new Control[] { _addButton, _editButton, _toggleButton });

            // Add controls to panel (grid first so top/bottom panels overlay correctly)
            Controls.Add(_grid);
            Controls.Add(_bottomPanel);
            Controls.Add(_topPanel);
        }
    }
}