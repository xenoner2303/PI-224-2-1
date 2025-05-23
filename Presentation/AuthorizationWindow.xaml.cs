using System.Windows;
using Presentation.UIHelpers;
using System.Windows.Controls;
using BLL.Commands.UsersManipulationCommands;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        private readonly IServiceProvider serviceProvider;
        private UserCommandsManager userCommandManager;

        public AuthorizationWindow(IServiceProvider serviceProvider, UserCommandsManager userCommandManager)
        {
            InitializeComponent();

            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.userCommandManager = userCommandManager ?? throw new ArgumentNullException(nameof(userCommandManager));
        }

        private void Authorization_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                var user = userCommandManager.AuthorizeUser(login, password);

                Window locUserWindow = WindowFactory.CreateWindow(user, serviceProvider);
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
            var registrationWindow = new RegistrationWindow(serviceProvider.GetRequiredService<UserCommandsManager>());
            registrationWindow.Show();
        }
    }
}
