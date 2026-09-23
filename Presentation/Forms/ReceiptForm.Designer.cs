namespace MiniMartManagement.Presentation.Forms
{
    partial class ReceiptForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label storeLabel;
        private System.Windows.Forms.Label metaLabel;

        private System.Windows.Forms.Panel itemsWrapper;
        private System.Windows.Forms.DataGridView itemsGrid;

        private System.Windows.Forms.TableLayoutPanel totalsPanel;

        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button printButton;
        private System.Windows.Forms.Button closeButton;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.headerPanel = new System.Windows.Forms.Panel();
            this.storeLabel = new System.Windows.Forms.Label();
            this.metaLabel = new System.Windows.Forms.Label();

            this.itemsWrapper = new System.Windows.Forms.Panel();
            this.itemsGrid = new System.Windows.Forms.DataGridView();

            this.totalsPanel = new System.Windows.Forms.TableLayoutPanel();

            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.printButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();

            // Form
            this.Text = "Receipt";
            this.ClientSize = new System.Drawing.Size(420, 640);
            this.MinimumSize = new System.Drawing.Size(400, 460);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;

            // headerPanel
            this.headerPanel.Dock = DockStyle.Top;
            this.headerPanel.Height = 108;
            this.headerPanel.Padding = new Padding(20, 14, 20, 14);
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);

            this.storeLabel.Text = "MiniMart";
            this.storeLabel.Dock = DockStyle.Top;
            this.storeLabel.Height = 30;
            this.storeLabel.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.storeLabel.ForeColor = System.Drawing.Color.White;

            this.metaLabel.Dock = DockStyle.Top;
            this.metaLabel.Height = 50;
            this.metaLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.metaLabel.ForeColor = System.Drawing.Color.LightGray;

            this.headerPanel.Controls.Add(this.metaLabel);
            this.headerPanel.Controls.Add(this.storeLabel);

            // totalsPanel
            this.totalsPanel.Dock = DockStyle.Bottom;
            this.totalsPanel.Height = 200;
            this.totalsPanel.Padding = new Padding(20, 14, 20, 14);
            this.totalsPanel.BackColor = System.Drawing.Color.FromArgb(250, 250, 252);
            this.totalsPanel.ColumnCount = 2;
            this.totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            this.totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            // buttonPanel
            this.buttonPanel.Dock = DockStyle.Bottom;
            this.buttonPanel.Height = 60;
            this.buttonPanel.Padding = new Padding(20, 12, 20, 12);
            this.buttonPanel.FlowDirection = FlowDirection.RightToLeft;

            this.printButton.Text = "Print";
            this.printButton.Size = new System.Drawing.Size(110, 34);

            this.closeButton.Text = "Close";
            this.closeButton.Size = new System.Drawing.Size(110, 34);

            this.buttonPanel.Controls.Add(this.closeButton);
            this.buttonPanel.Controls.Add(this.printButton);

            // itemsGrid
            this.itemsGrid.Dock = DockStyle.Fill;
            this.itemsGrid.ReadOnly = true;
            this.itemsGrid.AllowUserToAddRows = false;
            this.itemsGrid.AllowUserToDeleteRows = false;
            this.itemsGrid.AllowUserToResizeRows = false;
            this.itemsGrid.AutoGenerateColumns = false;
            this.itemsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.itemsGrid.BorderStyle = BorderStyle.None;
            this.itemsGrid.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "Product", HeaderText = "Product", Width = 180 });
            this.itemsGrid.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "Qty", HeaderText = "Qty", Width = 45 });
            this.itemsGrid.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "UnitPrice", HeaderText = "Unit", Width = 75 });
            this.itemsGrid.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "Subtotal", HeaderText = "Subtotal", Width = 75, DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle { Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight } });

            this.itemsWrapper.Dock = DockStyle.Fill;
            this.itemsWrapper.Padding = new Padding(20, 16, 20, 8);
            this.itemsWrapper.Controls.Add(this.itemsGrid);

            // Add to form (order matters: itemsWrapper fills center; totals and buttons dock bottom)
            this.Controls.Add(this.itemsWrapper);
            this.Controls.Add(this.totalsPanel);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.headerPanel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}