namespace MiniMartManagement.Presentation.Forms
{
    partial class ProductEditDialog
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel _layoutPanel;
        private System.Windows.Forms.PictureBox _imagePreview;
        private System.Windows.Forms.Button _chooseImageButton;
        private System.Windows.Forms.Button _removeImageButton;

        private System.Windows.Forms.Label _lblCategory;
        private System.Windows.Forms.ComboBox _categoryCombo;

        private System.Windows.Forms.Label _lblCode;
        private System.Windows.Forms.TextBox _codeBox;

        private System.Windows.Forms.Label _lblName;
        private System.Windows.Forms.TextBox _nameBox;

        private System.Windows.Forms.Label _lblPurchasePrice;
        private System.Windows.Forms.NumericUpDown _purchasePriceBox;

        private System.Windows.Forms.Label _lblSellingPrice;
        private System.Windows.Forms.NumericUpDown _sellingPriceBox;

        private System.Windows.Forms.Label _lblUnit;
        private System.Windows.Forms.TextBox _unitBox;

        private System.Windows.Forms.Label _lblMinStock;
        private System.Windows.Forms.NumericUpDown _minStockBox;

        private System.Windows.Forms.Label _errorLabel;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this._layoutPanel = new System.Windows.Forms.Panel();
            this._imagePreview = new System.Windows.Forms.PictureBox();
            this._chooseImageButton = new System.Windows.Forms.Button();
            this._removeImageButton = new System.Windows.Forms.Button();

            this._lblCategory = new System.Windows.Forms.Label();
            this._categoryCombo = new System.Windows.Forms.ComboBox();

            this._lblCode = new System.Windows.Forms.Label();
            this._codeBox = new System.Windows.Forms.TextBox();

            this._lblName = new System.Windows.Forms.Label();
            this._nameBox = new System.Windows.Forms.TextBox();

            this._lblPurchasePrice = new System.Windows.Forms.Label();
            this._purchasePriceBox = new System.Windows.Forms.NumericUpDown();

            this._lblSellingPrice = new System.Windows.Forms.Label();
            this._sellingPriceBox = new System.Windows.Forms.NumericUpDown();

            this._lblUnit = new System.Windows.Forms.Label();
            this._unitBox = new System.Windows.Forms.TextBox();

            this._lblMinStock = new System.Windows.Forms.Label();
            this._minStockBox = new System.Windows.Forms.NumericUpDown();

            this._errorLabel = new System.Windows.Forms.Label();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();

            // Form
            this.Text = "Product";
            this.ClientSize = new System.Drawing.Size(420, 560);
            this.MinimumSize = new System.Drawing.Size(420, 420);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AutoScroll = true;

            // layoutPanel
            this._layoutPanel.AutoSize = true;
            this._layoutPanel.Location = new System.Drawing.Point(15, 15);
            this._layoutPanel.Width = 380;
            this._layoutPanel.Padding = new System.Windows.Forms.Padding(0);

            int y = 0;

            // Photo controls
            this._imagePreview.Location = new System.Drawing.Point(0, y);
            this._imagePreview.Size = new System.Drawing.Size(96, 96);
            this._imagePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._imagePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this._chooseImageButton.Text = "Choose Image...";
            this._chooseImageButton.Location = new System.Drawing.Point(108, y + 6);
            this._chooseImageButton.Size = new System.Drawing.Size(130, 30);

            this._removeImageButton.Text = "Remove";
            this._removeImageButton.Location = new System.Drawing.Point(244, y + 6);
            this._removeImageButton.Size = new System.Drawing.Size(106, 30);

            y = 96 + 16;

            // Category
            this._lblCategory.Text = "Category";
            this._lblCategory.Location = new System.Drawing.Point(0, y);
            this._lblCategory.AutoSize = true;
            y += 20;
            this._categoryCombo.Location = new System.Drawing.Point(0, y);
            this._categoryCombo.Size = new System.Drawing.Size(350, 25);
            y += 25 + 14;

            // Product Code
            this._lblCode.Text = "Product Code";
            this._lblCode.Location = new System.Drawing.Point(0, y);
            this._lblCode.AutoSize = true;
            y += 20;
            this._codeBox.Location = new System.Drawing.Point(0, y);
            this._codeBox.Size = new System.Drawing.Size(350, 25);
            y += 25 + 14;

            // Product Name
            this._lblName.Text = "Product Name";
            this._lblName.Location = new System.Drawing.Point(0, y);
            this._lblName.AutoSize = true;
            y += 20;
            this._nameBox.Location = new System.Drawing.Point(0, y);
            this._nameBox.Size = new System.Drawing.Size(350, 25);
            y += 25 + 14;

            // Purchase Price
            this._lblPurchasePrice.Text = "Purchase Price";
            this._lblPurchasePrice.Location = new System.Drawing.Point(0, y);
            this._lblPurchasePrice.AutoSize = true;
            y += 20;
            this._purchasePriceBox.Location = new System.Drawing.Point(0, y);
            this._purchasePriceBox.Size = new System.Drawing.Size(350, 25);
            y += 25 + 14;

            // Selling Price
            this._lblSellingPrice.Text = "Selling Price";
            this._lblSellingPrice.Location = new System.Drawing.Point(0, y);
            this._lblSellingPrice.AutoSize = true;
            y += 20;
            this._sellingPriceBox.Location = new System.Drawing.Point(0, y);
            this._sellingPriceBox.Size = new System.Drawing.Size(350, 25);
            y += 25 + 14;

            // Unit
            this._lblUnit.Text = "Unit (e.g. bottle, pack)";
            this._lblUnit.Location = new System.Drawing.Point(0, y);
            this._lblUnit.AutoSize = true;
            y += 20;
            this._unitBox.Location = new System.Drawing.Point(0, y);
            this._unitBox.Size = new System.Drawing.Size(350, 25);
            y += 25 + 14;

            // Min stock
            this._lblMinStock.Text = "Minimum Stock Level";
            this._lblMinStock.Location = new System.Drawing.Point(0, y);
            this._lblMinStock.AutoSize = true;
            y += 20;
            this._minStockBox.Location = new System.Drawing.Point(0, y);
            this._minStockBox.Size = new System.Drawing.Size(350, 25);
            y += 25 + 14;

            // Error label
            this._errorLabel.Location = new System.Drawing.Point(0, y);
            this._errorLabel.Size = new System.Drawing.Size(350, 35);
            this._errorLabel.ForeColor = System.Drawing.Color.Firebrick;
            this._errorLabel.Text = string.Empty;
            y += 40;

            // Buttons
            this._okButton.Text = "Save";
            this._okButton.Location = new System.Drawing.Point(0, y);
            this._okButton.Size = new System.Drawing.Size(170, 34);

            this._cancelButton.Text = "Cancel";
            this._cancelButton.Location = new System.Drawing.Point(180, y);
            this._cancelButton.Size = new System.Drawing.Size(170, 34);

            // Add controls to layoutPanel
            this._layoutPanel.Controls.Add(this._imagePreview);
            this._layoutPanel.Controls.Add(this._chooseImageButton);
            this._layoutPanel.Controls.Add(this._removeImageButton);

            this._layoutPanel.Controls.Add(this._lblCategory);
            this._layoutPanel.Controls.Add(this._categoryCombo);

            this._layoutPanel.Controls.Add(this._lblCode);
            this._layoutPanel.Controls.Add(this._codeBox);

            this._layoutPanel.Controls.Add(this._lblName);
            this._layoutPanel.Controls.Add(this._nameBox);

            this._layoutPanel.Controls.Add(this._lblPurchasePrice);
            this._layoutPanel.Controls.Add(this._purchasePriceBox);

            this._layoutPanel.Controls.Add(this._lblSellingPrice);
            this._layoutPanel.Controls.Add(this._sellingPriceBox);

            this._layoutPanel.Controls.Add(this._lblUnit);
            this._layoutPanel.Controls.Add(this._unitBox);

            this._layoutPanel.Controls.Add(this._lblMinStock);
            this._layoutPanel.Controls.Add(this._minStockBox);

            this._layoutPanel.Controls.Add(this._errorLabel);
            this._layoutPanel.Controls.Add(this._okButton);
            this._layoutPanel.Controls.Add(this._cancelButton);

            // Add top-level panel to form
            this.Controls.Add(this._layoutPanel);
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