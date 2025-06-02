using DTOsLibrary;
using DTOsLibrary.DTOEnums;
using System.Windows;
using System.Windows.Controls;

namespace UI.Subcontrols
{
    public partial class SecretCodeDialog : Window
    {
        public SecretCodeDialog()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public SecretCodeRealizatorDto GetSecretCode()
        {
            var secretCode = InputSecretCodeBox.Text.Trim();

            if(string.IsNullOrWhiteSpace(secretCode))
            {
                MessageBox.Show("Секретний код не може бути порожнім", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            // отримуємо кількість використань з ComboBox + 0 за замовченням
            int uses = 1;
            if (cbUses.SelectedItem is ComboBoxItem selectedItem)
            {
                var content = selectedItem.Content.ToString();
                
                if (int.TryParse(content, out var parsedUses))
                {
                    uses = parsedUses;
                }
            }

            return new SecretCodeRealizatorDto
            {
                RealizatorType = cbType.SelectedIndex == 0
                    ? EnumInterfaceTypeDto.Manager
                    : EnumInterfaceTypeDto.Administrator,
                CodeUses = uses,
                SecretCode = secretCode
            };
        }
    }
}