using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ISIP422_Pluzhnikov_Egunov
{
    public partial class MainWindow : Window
    {
        private List<Product> products = new List<Product>();
        private int productCounter = 1;

        // Категории товаров
        private readonly List<string> categories = new List<string>
        {
            "Электроника",
            "Одежда",
            "Продукты",
            "Книги",
            "Спорт"
        };

        public MainWindow()
        {
            InitializeComponent();
            InitializeCategories();
            InitializeTestData();
            RefreshProductsList();
        }

        private void InitializeCategories()
        {
            SearchCategoryComboBox.Items.Clear();
            SearchCategoryComboBox.Items.Add(new ComboBoxItem { Content = "Все категории" });
            foreach (var category in categories)
            {
                SearchCategoryComboBox.Items.Add(new ComboBoxItem { Content = category });
            }
            SearchCategoryComboBox.SelectedIndex = 0;
        }

        private void InitializeTestData()
        {
            // Добавляем 5 тестовых товаров
            AddTestProduct("Ноутбук Lenovo", 45000, 10, "Электроника");
            AddTestProduct("Футболка мужская", 1500, 25, "Одежда");
            AddTestProduct("Молоко", 80, 100, "Продукты");
            AddTestProduct("Война и мир", 500, 15, "Книги");
            AddTestProduct("Футбольный мяч", 2500, 8, "Спорт");
        }

        private void AddTestProduct(string name, decimal price, int quantity, string category)
        {
            var product = new Product
            {
                Code = $"+{productCounter:D4}",
                Name = name,
                Price = price,
                Quantity = quantity,
                Category = category,
                IsAvailable = quantity > 0
            };
            products.Add(product);
            productCounter++;
        }

        private void RefreshProductsList(List<Product> filteredProducts = null)
        {
            var productsToShow = filteredProducts ?? products;
            ProductsDataGrid.ItemsSource = null;
            ProductsDataGrid.ItemsSource = productsToShow;
        }

        private void UpdateSelectedProductInfo()
        {
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                SelectedCodeText.Text = selectedProduct.Code;
                SelectedNameText.Text = selectedProduct.Name;
                SelectedPriceText.Text = $"{selectedProduct.Price:C}";
                SelectedQuantityText.Text = selectedProduct.Quantity.ToString();
                SelectedCategoryText.Text = selectedProduct.Category;
                SelectedStatusText.Text = selectedProduct.IsAvailable ? "В наличии" : "Нет в наличии";
            }
            else
            {
                ClearSelectedProductInfo();
            }
        }

        private void ClearSelectedProductInfo()
        {
            SelectedCodeText.Text = "";
            SelectedNameText.Text = "";
            SelectedPriceText.Text = "";
            SelectedQuantityText.Text = "";
            SelectedCategoryText.Text = "";
            SelectedStatusText.Text = "";
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProductDialog(categories, null);
            if (dialog.ShowDialog() == true)
            {
                var newProduct = new Product
                {
                    Code = $"+{productCounter:D4}",
                    Name = dialog.ProductName,
                    Price = dialog.ProductPrice,
                    Quantity = dialog.ProductQuantity,
                    Category = dialog.SelectedCategory,
                    IsAvailable = dialog.ProductQuantity > 0
                };

                products.Add(newProduct);
                productCounter++;
                RefreshProductsList();
                MessageBox.Show($"Товар '{newProduct.Name}' успешно добавлен!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить товар '{selectedProduct.Name}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    products.Remove(selectedProduct);
                    RefreshProductsList();
                    ClearSelectedProductInfo();
                    MessageBox.Show("Товар успешно удален!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите товар для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OrderSupply_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                var dialog = new QuantityDialog("Заказ поставки",
                    $"Введите количество для товара '{selectedProduct.Name}':");

                if (dialog.ShowDialog() == true)
                {
                    selectedProduct.Quantity += dialog.Quantity;
                    selectedProduct.IsAvailable = selectedProduct.Quantity > 0;
                    RefreshProductsList();
                    UpdateSelectedProductInfo();
                    MessageBox.Show($"Поставка товара '{selectedProduct.Name}' успешно зарегистрирована!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите товар для заказа поставки!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SellProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                if (!selectedProduct.IsAvailable)
                {
                    MessageBox.Show("Товара нет в наличии!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var dialog = new QuantityDialog("Продажа товара",
                    $"Введите количество для продажи товара '{selectedProduct.Name}':",
                    selectedProduct.Quantity);

                if (dialog.ShowDialog() == true)
                {
                    if (dialog.Quantity > selectedProduct.Quantity)
                    {
                        MessageBox.Show("Недостаточно товара на складе!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    selectedProduct.Quantity -= dialog.Quantity;
                    selectedProduct.IsAvailable = selectedProduct.Quantity > 0;
                    RefreshProductsList();
                    UpdateSelectedProductInfo();

                    decimal total = selectedProduct.Price * dialog.Quantity;
                    MessageBox.Show($"Товар '{selectedProduct.Name}' продан в количестве {dialog.Quantity} шт.\n" +
                                   $"Общая сумма: {total:C}", "Успех",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите товар для продажи!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearchFilter();
        }

        private void SearchCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            string searchText = SearchTextBox.Text.ToLower();
            string selectedCategory = (SearchCategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            var filteredProducts = products.Where(p =>
                (p.Code.ToLower().Contains(searchText) ||
                 p.Name.ToLower().Contains(searchText)) &&
                (selectedCategory == "Все категории" || p.Category == selectedCategory)
            ).ToList();

            RefreshProductsList(filteredProducts);
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            SearchCategoryComboBox.SelectedIndex = 0;
            RefreshProductsList();
        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedProductInfo();
        }
    }

    public class Product
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }
        public string Category { get; set; }
    }
}