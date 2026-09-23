namespace MiniMartManagement.Presentation.Forms
{
    public partial class StockAdjustDialog : Form
    {
        public int NewQuantity => (int)_quantityBox.Value;

        // Parameterless ctor for Designer
        public StockAdjustDialog()
        {
            InitializeComponent();

            UiTheme.StyleForm(this);
            UiTheme.StylePrimaryButton(_okButton);
            UiTheme.StyleSecondaryButton(_cancelButton);
            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }

        // Runtime ctor
        public StockAdjustDialog(string productName, int currentQuantity) : this()
        {
            Text = "Adjust Stock";
            _infoLabel.Text = $"{productName}\nCurrent quantity: {currentQuantity}";
            _quantityBox.Maximum = 1_000_000;
            _quantityBox.Value = currentQuantity;
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
