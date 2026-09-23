namespace MiniMartManagement.Presentation.Forms
{
    partial class StockAdjustDialog
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label _infoLabel;
        private System.Windows.Forms.Label _qtyLabel;
        private System.Windows.Forms.NumericUpDown _quantityBox;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this._infoLabel = new System.Windows.Forms.Label();
            this._qtyLabel = new System.Windows.Forms.Label();
            this._quantityBox = new System.Windows.Forms.NumericUpDown();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();

            // 
            // StockAdjustDialog (this)
            // 
            this.ClientSize = new System.Drawing.Size(320, 190);
            this.MinimumSize = new System.Drawing.Size(320, 190);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.AutoScroll = true;

            this._infoLabel.Location = new System.Drawing.Point(15, 15);
            this._infoLabel.Size = new System.Drawing.Size(290, 45);

            this._qtyLabel.Text = "New quantity";
            this._qtyLabel.Location = new System.Drawing.Point(15, 65);
            this._qtyLabel.AutoSize = true;

            this._quantityBox.Location = new System.Drawing.Point(15, 85);
            this._quantityBox.Size = new System.Drawing.Size(290, 25);
            this._quantityBox.Maximum = new decimal(new int[] {1000000,0,0,0});

            this._okButton.Text = "Save";
            this._okButton.Location = new System.Drawing.Point(15, 135);
            this._okButton.Size = new System.Drawing.Size(140, 32);
            this._okButton.Click += new System.EventHandler(this.OkButton_Click);

            this._cancelButton.Text = "Cancel";
            this._cancelButton.Location = new System.Drawing.Point(165, 135);
            this._cancelButton.Size = new System.Drawing.Size(140, 32);
            this._cancelButton.Click += new System.EventHandler(this.CancelButton_Click);

            this.Controls.Add(this._infoLabel);
            this.Controls.Add(this._qtyLabel);
            this.Controls.Add(this._quantityBox);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
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