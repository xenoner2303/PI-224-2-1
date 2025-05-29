using DTOsLibrary;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using UI.ApiClients;
using UI.UIHelpers;
using Presentation.UIHelpers;
using Presentation.UIHelpers.SubControls;
using UI;
using System.Windows.Controls;
using System.Windows.Media;

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
        private List<CategoryDto> flatCategoryList;
        private LotDemonstrationControl? selectedLotControl; // для нормального опрацювання лотів

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

        private async void PlaceBid_Click(object sender, RoutedEventArgs e)
        {
            // очищаємо повідомлення зауваження
            BidValidationLabel.Content = "";

            if (currentUser == null) // додатковий захист
            {
                MessageBox.Show("Будь ласка, авторизуйтесь перед створенням ставки");
                return;
            }

            if (selectedLotControl == null)
            {
                MessageBox.Show("Будь ласка, виберіть лот для здійснення ставки");
                return;
            }

            var input = BidTextBox.Text.Trim();

            if (!decimal.TryParse(input, out var bidAmount) || bidAmount <= 0)
            {
                BidValidationLabel.Content = "!";
                MessageBox.Show("Будь ласка, введіть коректну суму ставки");
                return;
            }

            var bid = new BidDto
            {
                Amount = bidAmount,
                PlacedAt = DateTime.Now,
                Lot = selectedLotControl.auctionLotDto,
                User = currentUser
            };

            var success = await client.CreateBidAsync(bid);

            if (success)
            {
                MessageBox.Show("Ставку успішно зроблено");
                BidTextBox.Text = ""; // очищуємо поле
            }
            else
            {
                MessageBox.Show("Не вдалося створити ставку");
            }
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            string? keyword = SearchBox.Text.Trim().ToLower();
            var selectedCategory = CategoryComboBox.SelectedItem as CategoryDto;

            SearchLotsDto search = new SearchLotsDto
            {
                Keyword = keyword,
                CategoryId = selectedCategory?.Id
            };

            var receiveLots = await client.SearchLotsAsync(search);

            if (receiveLots.Count == 0)
            {
                MainLotsCoursePanel.Children.Clear();

                MainLotsCoursePanel.Children.Add(new TextBlock
                {
                    Text = "Лоти не знайдено.",
                    Foreground = Brushes.Gray,
                    FontSize = 16,
                    Margin = new Thickness(10)
                });

                return;
            }

            FillLotsPanel(receiveLots);
        }

        private async void CreateLot_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser == null) // додатковий захист
            {
                MessageBox.Show("Будь ласка, авторизуйтесь перед створенням лота");
                return;
            }

            var lotCreationWindow = new LotCreationWindow(currentUser, flatCategoryList);
            lotCreationWindow.Owner = this; // встановлюємо це вікно господарем, щоб блокувалося поки не закрили вінко створення

            lotCreationWindow.ShowDialog();

            if (lotCreationWindow.DialogResult == true && lotCreationWindow.CreatedLot is AuctionLotDto newLot)
            {
                newLot.Owner = currentUser;

                var success = await client.CreateLotAsync(newLot);

                if (success)
                {
                    MessageBox.Show("Лот успішно створено");
                }
                else
                {
                    MessageBox.Show("Не вдалося створити лот");
                }
            }
        }

        private async void DeleteLot_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser == null)
            {
                MessageBox.Show("Будь ласка, авторизуйтесь перед видаленням лота");
                return;
            }

            if (selectedLotControl == null)
            {
                MessageBox.Show("Будь ласка, виберіть лот для видалення");
                return;
            }

            var lotId = selectedLotControl.auctionLotDto.Id;

            // попередження
            var confirmed = MessageBox.Show("Ви впевнені, що хочете видалити лот?", "Підтвердження", MessageBoxButton.YesNo);

            if (confirmed != MessageBoxResult.Yes)
            {
                return;
            }

            var success = await client.DeleteLotAsync(lotId);
            if (success)
            {
                MessageBox.Show("Лот видалено");
                UserLotsPanel.Children.Remove(selectedLotControl);
                selectedLotControl = null;
            }
            else
            {
                MessageBox.Show("Не вдалося видалити лот");
            }
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

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            currentUser = null;

            UserInfoPanel.Visibility = Visibility.Collapsed;
            LoginButton.Visibility = Visibility.Visible;
        }

        private void UpdateTabAccess()
        {
            bool userState = currentUser != null;

            // блокуємо вкладку "Мої лоти", якщо користувач неавторизований
            UserLotsTab.IsEnabled = userState;
            PlaceBidButton.IsEnabled = userState;

            if (userState)
            {
                LoginButton.Visibility = Visibility.Collapsed;
                UserNameText.Text = currentUser.Login;
                UserInfoPanel.Visibility = Visibility.Visible;
            }
        }

        private async void LoadUserManagerWindowEntities()
        {
            var categories = await client.GetCategoriesAsync();

            // Створення сплющеного списку категорій із нумерацією
            flatCategoryList = CategoryHelper.FlattenCategoriesWithNumbers(categories);

            // Використання UILoadHelper для заповнення ComboBox
            UILoadHelper.LoadEntities(flatCategoryList, CategoryComboBox, "DisplayName");

            if (currentUser != null)
            {
                var userLots = await client.GetUserLotsAsync(currentUser.Id);

                FillLotsPanel(userLots);
            }
        }

        private void FillLotsPanel(List<AuctionLotDto> lots)
        {
            UserLotsPanel.Children.Clear(); // очищаємо панель перед додаванням нових елементів

            foreach (var lot in lots)
            {
                var lotControl = new LotDemonstrationControl(lot);

                lotControl.LotSelected += (s, _) =>
                {
                    // якщо був виділений попередній — знімаємо з нього виділення
                    if (selectedLotControl != null)
                    {
                        selectedLotControl.IsSelected = false;
                    }

                    // зберігаємо новий виділений лот
                    selectedLotControl = (LotDemonstrationControl)s;
                    selectedLotControl.IsSelected = true;
                };

                UserLotsPanel.Children.Add(lotControl);
            }
        }
    }
}
