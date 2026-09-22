namespace MiniMartManagement.Presentation.Forms
{
    public class StockAdjustDialog : Form
    {
        private readonly NumericUpDown _quantityBox = new();

        public int NewQuantity => (int)_quantityBox.Value;

        public StockAdjustDialog(string productName, int currentQuantity)
        {
            UiTheme.StyleForm(this);
            Text = "Adjust Stock";
            ClientSize = new Size(320, 190);
            MinimumSize = new Size(320, 190);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScroll = true;

            Controls.Add(new Label
            {
                Text = $"{productName}\nCurrent quantity: {currentQuantity}",
                Location = new Point(15, 15),
                Size = new Size(290, 45)
            });

            Controls.Add(new Label { Text = "New quantity", Location = new Point(15, 65), AutoSize = true });
            _quantityBox.Location = new Point(15, 85);
            _quantityBox.Size = new Size(290, 25);
            _quantityBox.Maximum = 1_000_000;
            _quantityBox.Value = currentQuantity;
            Controls.Add(_quantityBox);

            var okButton = new Button { Text = "Save", Location = new Point(15, 135), Size = new Size(140, 32) };
            UiTheme.StylePrimaryButton(okButton);
            var cancelButton = new Button { Text = "Cancel", Location = new Point(165, 135), Size = new Size(140, 32) };
            UiTheme.StyleSecondaryButton(cancelButton);
            okButton.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };
            cancelButton.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(okButton);
            Controls.Add(cancelButton);
            AcceptButton = okButton;
            CancelButton = cancelButton;
        }
    }
}
