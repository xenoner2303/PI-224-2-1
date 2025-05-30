using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using UI.ApiClients;
using DTOsLibrary;
using DTOsLibrary.DTOEnums;
using Presentation.UIHelpers;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        private readonly IServiceProvider serviceProvider;
        private PreUserApiClient client;
        private readonly Action<BaseUserDto> onLoginSuccess;

        public AuthorizationWindow(IServiceProvider serviceProvider, PreUserApiClient client, Action<BaseUserDto> onLoginSuccess)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            ArgumentNullException.ThrowIfNull(onLoginSuccess, nameof(onLoginSuccess));

            InitializeComponent();

            this.serviceProvider = serviceProvider;
            this.client = client;
            this.onLoginSuccess = onLoginSuccess;
        }

        private async void Authorization_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                var userDto = await client.AuthorizeUserAsync(login, password);

                if (userDto.InterfaceType == EnumInterfaceTypeDto.Registered)
                {
                    onLoginSuccess?.Invoke(userDto); // передаємо назад користувача
                    this.Close();
                }

                Window locUserWindow = WindowFactory.CreateWindow(userDto, serviceProvider);
                locUserWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при авторизації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoRegistration_Click(object sender, RoutedEventArgs e)
        {
            // отримуємо RegistrationWindow через DI
            var registrationWindow = new RegistrationWindow(serviceProvider.GetRequiredService<PreUserApiClient>());
            registrationWindow.Show();
        }
    }
}
