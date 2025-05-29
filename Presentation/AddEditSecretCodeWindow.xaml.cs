using System;
using System.Windows;

namespace OnlineAuction
{
    public partial class AddEditSecretCodeWindow : Window
    {
        public SecretCode SecretCode { get; private set; }

        public AddEditSecretCodeWindow(SecretCode code = null)
        {
            InitializeComponent();

            if (code != null)
            {
                SecretCode = new SecretCode
                {
                    Id = code.Id,
                    Code = code.Code,
                    DiscountPercent = code.DiscountPercent,
                    ExpiryDate = code.ExpiryDate,
                    IsActive = code.IsActive
                };
                Title = "Редагувати секретний код";
            }
            else
            {
                SecretCode = new SecretCode
                {
                    Id = new Random().Next(100, 1000),
                    ExpiryDate = DateTime.Now.AddMonths(1),
                    IsActive = true
                };
                Title = "Додати новий секретний код";
            }

            DataContext = SecretCode;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SecretCode.Code))
            {
                MessageBox.Show("Будь ласка, введіть код", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SecretCode.DiscountPercent <= 0 || SecretCode.DiscountPercent > 100)
            {
                MessageBox.Show("Знижка повинна бути від 1% до 100%", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}