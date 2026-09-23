namespace MiniMartManagement.Presentation.Forms
{
    partial class DashboardForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel _sidebarPanel;
        private System.Windows.Forms.FlowLayoutPanel _sidebarFlowPanel;
        private System.Windows.Forms.Panel _contentPanel;
        private System.Windows.Forms.Panel _contentBody;
        private System.Windows.Forms.Label _pageTitleLabel;
        private System.Windows.Forms.Label _pageSubtitleLabel;

        /// <summary>Required method for Designer support — do not modify the contents with the code editor.</summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this._sidebarPanel = new System.Windows.Forms.Panel();
            this._sidebarFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._contentPanel = new System.Windows.Forms.Panel();
            this._contentBody = new System.Windows.Forms.Panel();
            this._pageTitleLabel = new System.Windows.Forms.Label();
            this._pageSubtitleLabel = new System.Windows.Forms.Label();

            // 
            // DashboardForm (this)
            // 
            this.ClientSize = new System.Drawing.Size(1100, 680);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MinimumSize = new System.Drawing.Size(860, 560);

            // 
            // _sidebarPanel
            // 
            this._sidebarPanel.SuspendLayout();
            this._sidebarPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this._sidebarPanel.Width = 240;
            this._sidebarPanel.BackColor = UiTheme.Success;
            this._sidebarPanel.Padding = new System.Windows.Forms.Padding(10);
            this._sidebarPanel.Controls.Add(this._sidebarFlowPanel);
            this._sidebarPanel.ResumeLayout(false);

            // 
            // _sidebarFlowPanel
            // 
            this._sidebarFlowPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._sidebarFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this._sidebarFlowPanel.BackColor = UiTheme.Success;
            this._sidebarFlowPanel.WrapContents = false;
            this._sidebarFlowPanel.AutoSize = true;

            // 
            // _contentPanel
            // 
            this._contentPanel.SuspendLayout();
            this._contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._contentPanel.Padding = new System.Windows.Forms.Padding(20);

            // Title area
            this._pageTitleLabel.Text = "Dashboard";
            this._pageTitleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this._pageTitleLabel.AutoSize = true;
            this._pageTitleLabel.ForeColor = System.Drawing.SystemColors.ControlText;

            this._pageSubtitleLabel.Text = string.Empty;
            this._pageSubtitleLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._pageSubtitleLabel.AutoSize = true;
            this._pageSubtitleLabel.ForeColor = System.Drawing.SystemColors.GrayText;

            var titleContainer = new System.Windows.Forms.Panel { Dock = System.Windows.Forms.DockStyle.Top, Height = 64, Padding = new System.Windows.Forms.Padding(0) };
            // Arrange subtitle above title so visual order matches original layout intent.
            this._pageSubtitleLabel.Location = new System.Drawing.Point(0, 8);
            this._pageTitleLabel.Location = new System.Drawing.Point(0, 30);
            titleContainer.Controls.Add(this._pageSubtitleLabel);
            titleContainer.Controls.Add(this._pageTitleLabel);

            // 
            // _contentBody
            // 
            this._contentBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this._contentBody.BackColor = System.Drawing.Color.White;

            this._contentPanel.Controls.Add(this._contentBody);
            this._contentPanel.Controls.Add(titleContainer);
            this._contentPanel.ResumeLayout(false);

            // Add to form (order: content then sidebar so sidebar appears left)
            this.Controls.Add(this._contentPanel);
            this.Controls.Add(this._sidebarPanel);
        }

        /// <summary>Clean up any resources being used.</summary>
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