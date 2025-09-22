using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ISIP422_Pluzhnikov_Egunov
{
    public partial class ProductDialog : Window
    {
        public string ProductName => NameTextBox.Text;
        public decimal ProductPrice { get; private set; }
        public int ProductQuantity { get; private set; }
        public string SelectedCategory => (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

        public ProductDialog(List<string> categories, Product existingProduct = null)
        {
            InitializeComponent();

            // Заполняем категории
            foreach (var category in categories)
            {
                CategoryComboBox.Items.Add(new ComboBoxItem { Content = category });
            }
            CategoryComboBox.SelectedIndex = 0;

            // Если редактируем существующий товар
            if (existingProduct != null)
            {
                Title = "Редактировать товар";
                NameTextBox.Text = existingProduct.Name;
                PriceTextBox.Text = existingProduct.Price.ToString();
                QuantityTextBox.Text = existingProduct.Quantity.ToString();

                var categoryItem = CategoryComboBox.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == existingProduct.Category);
                if (categoryItem != null)
                    CategoryComboBox.SelectedItem = categoryItem;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductName))
            {
                MessageBox.Show("Введите название товара!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Введите корректное количество!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ProductPrice = price;
            ProductQuantity = quantity;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}