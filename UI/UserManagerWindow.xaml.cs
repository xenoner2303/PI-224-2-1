using UI;
using DTOsLibrary;
using System.Windows;
using UI.ApiClients;
using Microsoft.Extensions.DependencyInjection;
using Presentation.UIHelpers.SubControls;
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

        // зробити ставку
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
                Search_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Не вдалося створити ставку");
            }
        }

        // знайти лоти за текстом та категорією
        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            string? keyword = SearchBox.Text.Trim().ToLower();
            var selectedCategory = CategoryTreeView.SelectedItem as CategoryDto;

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

            FillLotsPanel(receiveLots, MainLotsCoursePanel);
        }

        // створити лот
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
                var success = await client.CreateLotAsync(newLot);

                if (success)
                {
                    MessageBox.Show("Лот успішно створено");

                    var userLots = await client.GetUserLotsAsync(currentUser.Id);
                    FillLotsPanel(userLots, UserLotsPanel);
                }
                else
                {
                    MessageBox.Show("Не вдалося створити лот");
                }
            }
        }

        // видалити лот
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

        // перейти до авторизації (вікно юзера блокується вікном авторизації)
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

        // вийти з юзера
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            currentUser = null;

            UserLotsPanel.Children.Clear(); // очищаємо панель лотів юзера для безпеки

            UpdateTabAccess();
        }

        // допоміжний метод з налаштувань доступу до вкладок та кнопок зареєстрований/незареєстрований користувач
        private async void UpdateTabAccess()
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

                var userLots = await client.GetUserLotsAsync(currentUser.Id);
                FillLotsPanel(userLots, UserLotsPanel);
            }
            else
            {
                UserInfoPanel.Visibility = Visibility.Collapsed;
                LoginButton.Visibility = Visibility.Visible;
            }
        }

        // допоміжний метод із завантаження базових сутностей вікна
        private async void LoadUserManagerWindowEntities()
        {
            var categories = await client.GetCategoriesAsync();

            if (categories == null || categories.Count == 0)
            {
                CategoryTreeView.ItemsSource = null;
                CategoryTreeView.Items.Clear();
                CategoryTreeView.Items.Add(new TreeViewItem
                {
                    Header = new TextBlock
                    {
                        Text = "Категорії відсутні.",
                        Foreground = Brushes.Gray,
                        FontStyle = FontStyles.Italic
                    },
                    IsEnabled = false
                });
            }
            else
            {
                CategoryTreeView.ItemsSource = categories;
                flatCategoryList = categories;
            }
        }

        // допоміжний метод для заповнення панелі лотів
        private void FillLotsPanel(List<AuctionLotDto> lots, StackPanel panel)
        {
            panel.Children.Clear(); // очищаємо панель перед додаванням нових елементів

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

                panel.Children.Add(lotControl);
            }
        }
    }
}
