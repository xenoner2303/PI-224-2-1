using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DTOsLibrary;
using DTOsLibrary.DTOEnums;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Microsoft.Extensions.DependencyInjection;
using UI.Subcontrols;
using UI.ApiClients;

namespace UI
{
    public partial class AdminWindow : Window
    {
        private readonly AdministratorApiClient _adminApiClient;
        private List<BaseUserDto> _users = new List<BaseUserDto>();
        private List<SecretCodeRealizatorDto> _secretCodes = new List<SecretCodeRealizatorDto>();
        private List<ActionLogDto> _logs = new List<ActionLogDto>();
        private List<AuctionLotDto> _auctionLots = new List<AuctionLotDto>();
        private IServiceProvider _serviceProvider;

        private enum AdminSection { Users, SecretCodes, Logs, Report }
        private AdminSection _currentSection = AdminSection.Users;

        public AdminWindow(AdministratorApiClient adminApiClient, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _adminApiClient = adminApiClient;
            Loaded += AdminWindow_Loaded;
            _serviceProvider = serviceProvider;
        }

        private async void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadInitialDataAsync();
            ShowUsersSection();
        }

        private async Task LoadInitialDataAsync()
        {
            try
            {
                _users = await _adminApiClient.GetUsersAsync();
                _secretCodes = await _adminApiClient.GetCodeRealizatorsAsync();
                _logs = await _adminApiClient.GetLogsAsync(null);
                _auctionLots = await _adminApiClient.GetAuctionLotsAsync();

                foreach (var lot in _auctionLots)
                {
                    if (lot.Bids != null && lot.Bids.Any())
                    {
                        var highestBid = lot.Bids.MaxBy(b => b.Amount);
                        lot.Bids = new List<BidDto> { highestBid }; // залишаємо лише найвищу ставку для звільнення місця
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowUsersSection()
        {
            _currentSection = AdminSection.Users;
            tbCurrentSection.Text = "Керування користувачами";

            dgMainData.Visibility = Visibility.Visible;

            dgMainData.IsReadOnly = false;
            dgMainData.ItemsSource = _users;

            dgMainData.Columns.Clear();
            dgMainData.AutoGenerateColumns = false;

            dgMainData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id") });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Логін", Binding = new Binding("Login") });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Ім'я", Binding = new Binding("FirstName") });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Прізвище", Binding = new Binding("LastName") });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Пароль", Binding = new Binding("Password") });
            // буде засекреченим навіть для адміністратора через налаштування профілю автомаперу
            dgMainData.Columns.Add(new DataGridComboBoxColumn { Header = "Роль", SelectedItemBinding = new Binding("InterfaceType") });

            // заповюнємо комбобокс ролей значеннями для динамічноїї зміни та відображення
            var comboBoxColumn = dgMainData.Columns
                .OfType<DataGridComboBoxColumn>()
                .FirstOrDefault(c => c.Header?.ToString() == "Роль");

            if (comboBoxColumn != null)
            {
                comboBoxColumn.ItemsSource = Enum.GetValues(typeof(EnumInterfaceTypeDto));
            }

            btnDelete.Visibility = Visibility.Visible; // кнопка видалення активна для користувачів
            btnEdit.Visibility = Visibility.Visible;
            LogDatePicker.Visibility = Visibility.Collapsed;
            LogDateText.Visibility = Visibility.Collapsed;
            btnShowLogs.Visibility = Visibility.Collapsed;
            statsBorder.Visibility = Visibility.Collapsed;
            btnAdd.Visibility = Visibility.Collapsed; // Кнопка "Додати" не потрібна для користувачів
            UpdateNavButtonsStyle();
        }

        private void ShowSecretCodesSection()
        {
            _currentSection = AdminSection.SecretCodes;
            tbCurrentSection.Text = "Секретні коди доступу";

            dgMainData.Visibility = Visibility.Visible;
            dgMainData.IsReadOnly = true;

            dgMainData.ItemsSource = _secretCodes;

            dgMainData.Columns.Clear();
            dgMainData.AutoGenerateColumns = false;

            dgMainData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id") });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Тип", Binding = new Binding("RealizatorType") });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Використань", Binding = new Binding("CodeUses") });

            btnDelete.Visibility = Visibility.Visible;
            LogDatePicker.Visibility = Visibility.Collapsed;
            LogDateText.Visibility = Visibility.Collapsed;
            btnShowLogs.Visibility = Visibility.Collapsed;
            statsBorder.Visibility = Visibility.Collapsed;
            btnAdd.Visibility = Visibility.Visible; // кнопка "Додати" активна для кодів
            btnEdit.Visibility = Visibility.Collapsed; // редагування кодів не передбачено
            UpdateNavButtonsStyle();
        }

        private void ShowLogsSection()
        {
            _currentSection = AdminSection.Logs;
            tbCurrentSection.Text = "Журнал логів";

            dgMainData.Visibility = Visibility.Visible;
            dgMainData.IsReadOnly = true;

            dgMainData.ItemsSource = _logs;

            dgMainData.Columns.Clear();
            dgMainData.AutoGenerateColumns = false;

            dgMainData.Columns.Add(new DataGridTextColumn
            {
                Header = "Час",
                Binding = new Binding("ActionTime") { StringFormat = "dd.MM.yyyy HH:mm" }
            });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Дія", Binding = new Binding("ActionName") });
            dgMainData.Columns.Add(new DataGridTextColumn
            {
                Header = "Опис",
                Binding = new Binding("Description"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            LogDatePicker.Visibility = Visibility.Visible; // показуємо календар для фільтрації логів
            LogDateText.Visibility = Visibility.Visible; // показуємо текст для вибору дати
            btnShowLogs.Visibility = Visibility.Visible; // кнопка для фільтрації логів за датою
            statsBorder.Visibility = Visibility.Collapsed;
            btnAdd.Visibility = Visibility.Collapsed; // кнопка "Додати" не потрібна для логів
            btnDelete.Visibility = Visibility.Collapsed; // видалення логів не передбачено
            UpdateNavButtonsStyle();
        }

        private void ShowReportSection()
        {
            _currentSection = AdminSection.Report;
            tbCurrentSection.Text = "Звітність акціону";

            btnEdit.Visibility = Visibility.Collapsed; // редагування не потрібне в звіті
            dgMainData.Visibility = Visibility.Collapsed; // приховуємо таблицю, оскільки звіт не прив'язаний до датасітки
            LogDatePicker.Visibility = Visibility.Collapsed; // показуємо календар для фільтрації логів
            LogDateText.Visibility = Visibility.Collapsed; // показуємо текст для вибору дати
            btnShowLogs.Visibility = Visibility.Collapsed; // кнопка для фільтрації логів за датою
            btnAdd.Visibility = Visibility.Collapsed; // кнопка "Додати" не потрібна для логів
            btnDelete.Visibility = Visibility.Collapsed; // видалення логів не передбачено

            FillReportControls();
            statsBorder.Visibility = Visibility.Visible;
            UpdateNavButtonsStyle();
        }

        private void UpdateNavButtonsStyle()
        {
            // Скидаємо стилі всіх кнопок навігації
            btnUsers.Style = (Style)FindResource("ModernButton");
            btnSecretCodes.Style = (Style)FindResource("ModernButton");
            btnLogs.Style = (Style)FindResource("ModernButton");
            btnReport.Style = (Style)FindResource("ModernButton");

            // Встановлюємо активний стиль для поточної кнопки
            switch (_currentSection)
            {
                case AdminSection.Users:
                    btnUsers.Style = (Style)FindResource("ActiveNavButton");
                    break;
                case AdminSection.SecretCodes:
                    btnSecretCodes.Style = (Style)FindResource("ActiveNavButton");
                    break;
                case AdminSection.Logs:
                    btnLogs.Style = (Style)FindResource("ActiveNavButton");
                    break;
                case AdminSection.Report:
                    btnReport.Style = (Style)FindResource("ActiveNavButton");
                    break;
            }
        }

        private void FillReportControls()
        {
            // налаштовуємо ліво-вверх звітності про користувачів
            TotalUserCount.Text = $"Загальна кількість користувачів: {_users.Count.ToString()}";
            UserPieChart.Series = new ISeries[]
            {
                new PieSeries<int> 
                {
                    Values = [_users.Count(x => x.InterfaceType == EnumInterfaceTypeDto.Registered)],
                    Name = "Звичайні користувачі" },
                new PieSeries<int>
                {
                    Values = [_users.Count(x => x.InterfaceType == EnumInterfaceTypeDto.Manager)],
                    Name = "Менеджери" },
                new PieSeries<int>
                {
                    Values = [_users.Count(x => x.InterfaceType == EnumInterfaceTypeDto.Administrator)],
                    Name = "Адміністратори" }
            };

            // налаштовуємо праву-вверх звітності про капітал
            decimal totalHighestBids = 0;
            decimal totalActiveHighestBids = 0;
            decimal totalCompleteHighestBids = 0;

            foreach (var lot in _auctionLots)
            {
                if (lot.Bids != null && lot.Bids.Any())
                {
                    totalHighestBids += lot.Bids[0].Amount; // бо ми вже зберегли лише найвищу ставку

                    if (lot.Status == EnumLotStatusesDto.Active)
                    {
                        totalActiveHighestBids += lot.Bids[0].Amount;
                    }
                    else if (lot.Status == EnumLotStatusesDto.Completed)
                    {
                        totalCompleteHighestBids += lot.Bids[0].Amount;
                    }
                }
            }

            TotalCapital.Text = $"Загальний капітал аукціону: {totalHighestBids.ToString()}"; // відображаємо загальний капітал у форматі валюти

            CapitalChart.Series = new ISeries[]
            {
                new PieSeries<decimal> 
                { 
                    Values = [totalActiveHighestBids], // зручний синтаксичний цукор 
                    Name = "Загальний активний капітал" },
                new PieSeries<decimal> 
                { 
                    Values = [totalCompleteHighestBids],
                    Name = "Загальний пасивний капітал" }
            };

            // налаштовуємо ліво-вниз звітності про програму по логам
            CreateDataText.Text = $"Початок роботи програми за логами: {_logs.MinBy(b => b.ActionTime)?.ActionTime.ToString()}";
            LastDataText.Text = $"Останній день роботи програми за логами: {_logs.MaxBy(b => b.ActionTime)?.ActionTime.ToString()}";

            // групуємо логи по даті (без часу)
            var logsByDate = _logs.GroupBy(log => log.ActionTime.Date).ToList();
            var mostActiveDay = logsByDate.OrderByDescending(g => g.Count()).FirstOrDefault();
            var leastActiveDay = logsByDate.OrderBy(g => g.Count()).FirstOrDefault();

            MostActivityText.Text = "Найбільш активний лень: " + (mostActiveDay != null
                ? $"{mostActiveDay.Key:yyyy-MM-dd} ({mostActiveDay.Count()} логів)"
                : "немає даних");

            LeastActivityText.Text = "Найменш активний лень: " + (leastActiveDay != null
                ? $"{leastActiveDay.Key:yyyy-MM-dd} ({leastActiveDay.Count()} логів)"
                : "немає даних");

            // налаштовуємо право-вниз звітності саме про лоти
            TotalLotsText.Text = $"Загальна кількість лотів: {_auctionLots.Count.ToString()}";

            var lotWithBiggestBid = _auctionLots
                .Where(lot => lot.Bids != null && lot.Bids.Count > 0)
                .OrderByDescending(lot => lot.Bids[0].Amount)
                .FirstOrDefault();

            BiggestBidLotText.Text = "Власник: " + (lotWithBiggestBid != null ?
                $"{lotWithBiggestBid.Owner} Лот: {lotWithBiggestBid.Title} Ставка: {lotWithBiggestBid.Bids[0].Amount}"
                : "немає даних");

            var lotWithLowestBid = _auctionLots
                .Where(lot => lot.Bids != null && lot.Bids.Count > 0)
                .OrderBy(lot => lot.Bids[0].Amount)
                .FirstOrDefault();

            LowestBidLotText.Text = "Власник: " + (lotWithLowestBid != null ?
                $"{lotWithLowestBid.Owner} Лот: {lotWithLowestBid.Title} Ставка: {lotWithLowestBid.Bids[0].Amount}"
                : "немає даних");

            var statuses = Enum.GetValues(typeof(EnumLotStatusesDto)).Cast<EnumLotStatusesDto>();
            var series = new List<ISeries>();

            foreach (var status in statuses)
            {
                int count = _auctionLots.Count(lot => lot.Status == status);

                //додаємо PieSeries з кількістю лотів для цього статусу
                series.Add(new PieSeries<int>
                {
                    Values = [count],
                    Name = status.ToString()
                });
            }

            LotTypeChart.Series = series;
        }

        // ========== Обробники подій ==========
        private void btnReport_Click(object sender, RoutedEventArgs e) => ShowReportSection();
        private void btnSecretCodes_Click(object sender, RoutedEventArgs e) => ShowSecretCodesSection();
        private void btnLogs_Click(object sender, RoutedEventArgs e) => ShowLogsSection();
        private void btnUsers_Click(object sender, RoutedEventArgs e) => ShowUsersSection();

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (_currentSection)
                {
                    case AdminSection.SecretCodes:
                        var codeDialog = new SecretCodeDialog();
                        if (codeDialog.ShowDialog() == true)
                        {
                            var newCode = codeDialog.GetSecretCode();
                            var result = await _adminApiClient.CreateCodeRealizatorAsync(newCode);

                            if (result)
                            {
                                _secretCodes = await _adminApiClient.GetCodeRealizatorsAsync();
                                dgMainData.ItemsSource = _secretCodes;
                                MessageBox.Show("Новий код доступу успішно створений", "Успіх",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при додаванні: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgMainData.SelectedItem == null)
            {
                MessageBox.Show("Виберіть елемент для редагування", "Попередження",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                switch (_currentSection)
                {
                    case AdminSection.Users:
                        {
                            if (dgMainData.ItemsSource is List<BaseUserDto> usersEnumerable)
                            {
                                var result = await _adminApiClient.UpdateUsersAsync(usersEnumerable);

                                if (result)
                                {
                                    _users = await _adminApiClient.GetUsersAsync();
                                    dgMainData.ItemsSource = _users;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Не вдалося отримати список користувачів або невірний формат даних", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при редагуванні: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgMainData.SelectedItem == null)
            {
                MessageBox.Show("Виберіть елемент для видалення", "Попередження",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool result = false;
                string successMessage = "";

                switch (_currentSection)
                {
                    case AdminSection.Users:
                        var user = (BaseUserDto)dgMainData.SelectedItem;
                        result = await _adminApiClient.DeleteUserAsync(user.Id);
                        successMessage = $"Користувач {user.Login} успішно видалений";
                        _users.Remove(user);
                        break;

                    case AdminSection.SecretCodes:
                        var code = (SecretCodeRealizatorDto)dgMainData.SelectedItem;
                        result = await _adminApiClient.DeleteCodeRealizatorAsync(code.Id);
                        successMessage = $"Код доступу типу {code.RealizatorType} успішно видалений";
                        _secretCodes.Remove(code);
                        break;
                }

                if (result)
                {
                    MessageBox.Show(successMessage, "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                    dgMainData.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при видаленні: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow? authWindow = null;

            try
            {
                var client = _serviceProvider.GetRequiredService<PreUserApiClient>();

                authWindow = new AuthorizationWindow(_serviceProvider, client);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при кроку авторизації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            authWindow!.Show(); // визиваємо окремо, щоб не було зайвого обгортання та навантаження на програму
        }

        private void dgMainData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEdit.IsEnabled = dgMainData.SelectedItem != null;
            btnDelete.IsEnabled = dgMainData.SelectedItem != null;
        }

        private async void ShowLogs_Click(object sender, RoutedEventArgs e)
        {
            var selectedDate = LogDatePicker.SelectedDate;

            try
            {
                _logs = await _adminApiClient.GetLogsAsync(selectedDate);
                dgMainData.ItemsSource = _logs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при отриманні логів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}