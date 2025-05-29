using DTOsLibrary;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using UI.ApiClients;
using UI.UIHelpers;
using Presentation.UIHelpers;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for UserManagerWindow.xaml
    /// </summary>
    public partial class UserManagerWindow : Window
    {
        private BaseUserDto? currentUser;
        private readonly IServiceProvider serviceProvider;
        private readonly UserApiClient client;

        public UserManagerWindow(IServiceProvider serviceProvider, UserApiClient client)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
            ArgumentNullException.ThrowIfNull(client, nameof(client));

            InitializeComponent();

            this.serviceProvider = serviceProvider;
            this.client = client;
            UpdateTabAccess();
            LoadUserManagerWindowEntities();
        }

        private void PlaceBid_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateLot_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteLot_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var client = serviceProvider.GetRequiredService<PreUserApiClient>();

            var authWindow = new AuthorizationWindow(serviceProvider, client, user =>
            {
                this.currentUser = user;
                UpdateTabAccess(); // оновлюємо доступ до елементів
            });

            authWindow.Owner = this; // ставимо власником це вікно + щоб блокувалося якщо використовується ShowDialog()
            authWindow.ShowDialog();
        }

        private void UpdateTabAccess()
        {
            bool userState = currentUser != null;

            // блокуємо вкладку "Мої лоти", якщо користувач неавторизований
            UserLotsTab.IsEnabled = userState;
            PlaceBidButton.IsEnabled = userState;
        }

        private async void LoadUserManagerWindowEntities()
        {
            var categories = await client.GetCategoriesAsync();

            // Створення сплющеного списку категорій із нумерацією
            var flatList = CategoryHelper.FlattenCategoriesWithNumbers(categories);

            // Використання UILoadHelper для заповнення ComboBox
            UILoadHelper.LoadEntities(flatList, CategoryComboBox, "DisplayName");

            if (currentUser != null)
            {
                // zкщо користувач авторизований, завантажуємо його лоти
                var userLots = await client.GetUserLotsAsync(currentUser.Id);

            }
        }
    }
}
