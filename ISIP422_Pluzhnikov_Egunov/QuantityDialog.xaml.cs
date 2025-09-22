using System.Windows;

namespace ISIP422_Pluzhnikov_Egunov
{
    public partial class QuantityDialog : Window
    {
        public int Quantity { get; private set; }

        public QuantityDialog(string title, string message, int maxQuantity = int.MaxValue)
        {
            InitializeComponent();
            Title = title;
            MessageText.Text = message;
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Quantity = quantity;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}