using DTOsLibrary;
using System.Windows;
using System.Windows.Controls;
using UI.ApiClients;

namespace UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private bool isLoaded = false;
        private readonly PreUserApiClient client;

        public RegistrationWindow(PreUserApiClient client)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));

            InitializeComponent();

            this.client = client;
        }

        // обробка події завантаження вікна (для безпеки що обрано саме перший крок реєстрації)
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;
            StepsListBox.SelectedIndex = 0;
        }

        // обробка зміни вибору в списку кроків реєстрації
        private void StepsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isLoaded) return;

            PersonalInfoPanel.Visibility = Visibility.Collapsed;
            ContactInfoPanel.Visibility = Visibility.Collapsed;
            PasswordPanel.Visibility = Visibility.Collapsed;

            switch (StepsListBox.SelectedIndex)
            {
                case 0:
                    PersonalInfoPanel.Visibility = Visibility.Visible;
                    BackButton.Visibility = Visibility.Collapsed;
                    NextButton.Visibility = Visibility.Visible;
                    RegisterButton.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    ContactInfoPanel.Visibility = Visibility.Visible;
                    BackButton.Visibility = Visibility.Visible;
                    NextButton.Visibility = Visibility.Visible;
                    RegisterButton.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    PasswordPanel.Visibility = Visibility.Visible;
                    BackButton.Visibility = Visibility.Visible;
                    NextButton.Visibility = Visibility.Collapsed;
                    RegisterButton.Visibility = Visibility.Visible;
                    break;
            }
        }

        // обробка кнопок "Назад" та "Далі" для навігації між кроками реєстрації
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (StepsListBox.SelectedIndex > 0)
            {
                StepsListBox.SelectedIndex--;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int maxIndex = StepsListBox.Items.Count - 1;
            if (StepsListBox.SelectedIndex < maxIndex)
            {
                StepsListBox.SelectedIndex++;
            }
        }

        // обробка кнопки "Зареєструватися" для завершення реєстрації
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userDto = new BaseUserDto
                {
                    FirstName = NameTextBox.Text.Trim(),
                    LastName = LastNameTextBox.Text.Trim(),
                    Age = int.TryParse(AgeTextBox.Text, out var age) ? age : throw new ArgumentException("Вік має бути числом"),
                    Login = LoginTextBox.Text.Trim(),
                    Email = EmailTextBox.Text.Trim(),
                    PhoneNumber = PhoneNumberTextBox.Text.Trim(),
                    Password = PasswordBox0.Password.Trim() == PasswordBox1.Password.Trim()
                        ? PasswordBox0.Password.Trim()
                        : throw new ArgumentException("Паролі не збігаються"),
                    SecretCode = SecretKeyBox.Password.Trim()
                };

                bool created = await client.CreateUserAsync(userDto);

                if (created)
                {
                    MessageBox.Show("Користувач успішно зареєстрований!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при реєстрації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
