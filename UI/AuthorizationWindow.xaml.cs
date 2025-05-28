using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using UI.ApiClients;

namespace Presentation
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

            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        private async void Authorization_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                var userDto = await client.AuthorizeUserAsync(login, password);

             //   Window locUserWindow = WindowFactory.CreateWindow(user, serviceProvider);
               // locUserWindow.Show();
             //   this.Close();
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
