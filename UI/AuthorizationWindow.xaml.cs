using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using UI.ApiClients;
using DTOsLibrary;
using DTOsLibrary.DTOEnums;
using UI.UIHelpers;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        private readonly IServiceProvider serviceProvider;
        private PreUserApiClient client;

        public AuthorizationWindow(IServiceProvider serviceProvider, PreUserApiClient client)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
            ArgumentNullException.ThrowIfNull(client, nameof(client));

            InitializeComponent();

            this.serviceProvider = serviceProvider;
            this.client = client;
        }

        // обробка кнопки "Вхід" для завершення авторизації
        private async void Authorization_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                var userDto = await client.AuthorizeUserAsync(login, password);

                if (userDto == null)
                {
                    return;
                }

                // створюємо вікно потрібного типу якщо це не визов типу callback
                Window locUserWindow = WindowFactory.CreateWindow(userDto, serviceProvider);
                locUserWindow.Show();

                if (this.Owner is Window ownerWindow)
                {
                    ownerWindow.Close();
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при авторизації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // перехід до вікна реєстрації
        private void GoRegistration_Click(object sender, RoutedEventArgs e)
        {
            // отримуємо RegistrationWindow через DI
            var registrationWindow = new RegistrationWindow(serviceProvider.GetRequiredService<PreUserApiClient>());
            registrationWindow.Show();
        }

        private void ContinueAsGuest_Click(object sender, RoutedEventArgs e)
        {
            var notRegisteredUserWindow = new UserManagerWindow(null, serviceProvider,
                serviceProvider.GetRequiredService<UserApiClient>());

            notRegisteredUserWindow.Show();
            this.Close();
        }
    }
}
