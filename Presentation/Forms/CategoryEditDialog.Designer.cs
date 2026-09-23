namespace MiniMartManagement.Presentation.Forms
{
    partial class CategoryEditDialog
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox _nameBox;
        private System.Windows.Forms.TextBox _descriptionBox;
        private System.Windows.Forms.Label _errorLabel;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Label _nameLabel;
        private System.Windows.Forms.Label _descriptionLabel;

        /// <summary>Required method for Designer support — do not modify the contents with the code editor.</summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this._nameBox = new System.Windows.Forms.TextBox();
            this._descriptionBox = new System.Windows.Forms.TextBox();
            this._errorLabel = new System.Windows.Forms.Label();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._nameLabel = new System.Windows.Forms.Label();
            this._descriptionLabel = new System.Windows.Forms.Label();

            // 
            // CategoryEditDialog (this)
            // 
            this.Text = "Category";
            this.ClientSize = new System.Drawing.Size(340, 260);
            this.MinimumSize = new System.Drawing.Size(340, 260);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AutoScroll = true;

            // 
            // _nameLabel
            // 
            this._nameLabel.Text = "Category Name";
            this._nameLabel.Location = new System.Drawing.Point(15, 15);
            this._nameLabel.AutoSize = true;

            // 
            // _nameBox
            // 
            this._nameBox.Location = new System.Drawing.Point(15, 35);
            this._nameBox.Size = new System.Drawing.Size(300, 25);

            // 
            // _descriptionLabel
            // 
            this._descriptionLabel.Text = "Description (optional)";
            this._descriptionLabel.Location = new System.Drawing.Point(15, 70);
            this._descriptionLabel.AutoSize = true;

            // 
            // _descriptionBox
            // 
            this._descriptionBox.Location = new System.Drawing.Point(15, 90);
            this._descriptionBox.Size = new System.Drawing.Size(300, 60);
            this._descriptionBox.Multiline = true;

            // 
            // _errorLabel
            // 
            this._errorLabel.Location = new System.Drawing.Point(15, 155);
            this._errorLabel.Size = new System.Drawing.Size(300, 20);
            this._errorLabel.ForeColor = System.Drawing.Color.Firebrick;
            this._errorLabel.AutoSize = false;
            this._errorLabel.Text = string.Empty;

            // 
            // _okButton
            // 
            this._okButton.Text = "Save";
            this._okButton.Location = new System.Drawing.Point(15, 200);
            this._okButton.Size = new System.Drawing.Size(140, 32);
            this._okButton.Click += new System.EventHandler(this.OkButton_Click);

            // 
            // _cancelButton
            // 
            this._cancelButton.Text = "Cancel";
            this._cancelButton.Location = new System.Drawing.Point(175, 200);
            this._cancelButton.Size = new System.Drawing.Size(140, 32);
            this._cancelButton.Click += new System.EventHandler(this.CancelButton_Click);

            // Add controls to form
            this.Controls.Add(this._nameLabel);
            this.Controls.Add(this._nameBox);
            this.Controls.Add(this._descriptionLabel);
            this.Controls.Add(this._descriptionBox);
            this.Controls.Add(this._errorLabel);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
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