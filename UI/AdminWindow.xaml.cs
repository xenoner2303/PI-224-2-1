using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DTOsLibrary;
using DTOsLibrary.DTOEnums;
using Microsoft.Extensions.DependencyInjection;
using Presentation;
using UI.ApiClients;
using static MaterialDesignThemes.Wpf.Theme;

namespace OnlineAuction
{
    public partial class AdminWindow : Window
    {
        private readonly AdministratorApiClient _adminApiClient;
        private List<BaseUserDto> _users = new List<BaseUserDto>();
        private List<SecretCodeRealizatorDto> _secretCodes = new List<SecretCodeRealizatorDto>();
        private List<ActionLogDto> _logs = new List<ActionLogDto>();

        private enum AdminSection { Users, SecretCodes, Logs }
        private AdminSection _currentSection = AdminSection.Users;

        public AdminWindow(AdministratorApiClient adminApiClient)
        {
            InitializeComponent();
            _adminApiClient = adminApiClient;
            Loaded += AdminWindow_Loaded;
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

            LogDatePicker.Visibility = Visibility.Collapsed;
            LogDateText.Visibility = Visibility.Collapsed;
            btnShowLogs.Visibility = Visibility.Collapsed;
            statsPanel.Visibility = Visibility.Visible;
            btnAdd.Visibility = Visibility.Collapsed; // Кнопка "Додати" не потрібна для користувачів
            UpdateStatsPanel();
            UpdateNavButtonsStyle();
        }

        private void ShowSecretCodesSection()
        {
            _currentSection = AdminSection.SecretCodes;
            tbCurrentSection.Text = "Секретні коди доступу";
            dgMainData.ItemsSource = _secretCodes;

            dgMainData.Columns.Clear();
            dgMainData.AutoGenerateColumns = false;

            dgMainData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id") });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Тип", Binding = new Binding("RealizatorType") });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Використань", Binding = new Binding("CodeUses") });

            LogDatePicker.Visibility = Visibility.Collapsed;
            LogDateText.Visibility = Visibility.Collapsed;
            btnShowLogs.Visibility = Visibility.Collapsed;
            statsPanel.Visibility = Visibility.Collapsed;
            btnAdd.Visibility = Visibility.Visible; // кнопка "Додати" активна для кодів
            btnEdit.Visibility = Visibility.Collapsed; // редагування кодів не передбачено
            UpdateNavButtonsStyle();
        }

        private void ShowLogsSection()
        {
            _currentSection = AdminSection.Logs;
            tbCurrentSection.Text = "Журнал логів";
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
            statsPanel.Visibility = Visibility.Collapsed;
            btnAdd.Visibility = Visibility.Collapsed; // кнопка "Додати" не потрібна для логів
            btnDelete.Visibility = Visibility.Collapsed; // видалення логів не передбачено
            UpdateNavButtonsStyle();
        }

        private void UpdateStatsPanel()
        {
            var adminCount = _users.Count(u => u.InterfaceType == EnumInterfaceTypeDto.Administrator);
            var managerCount = _users.Count(u => u.InterfaceType == EnumInterfaceTypeDto.Manager);
            var userCount = _users.Count(u => u.InterfaceType == EnumInterfaceTypeDto.Registered);

            statsPanel.Children.Clear();
            statsPanel.Children.Add(new TextBlock
            {
                Text = $"Адміністраторів: {adminCount}\nМенеджерів: {managerCount}\nКористувачів: {userCount}",
                Margin = new Thickness(0, 10, 0, 0),
                FontSize = 14
            });
        }

        private void UpdateNavButtonsStyle()
        {
            // Скидаємо стилі всіх кнопок навігації
            btnLots.Style = (Style)FindResource("ModernButton");
            btnUsers.Style = (Style)FindResource("ModernButton");
            btnCategories.Style = (Style)FindResource("ModernButton");
            btnSecretCodes.Style = (Style)FindResource("ModernButton");
            btnLogs.Style = (Style)FindResource("ModernButton");

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
            }
        }

        // ========== Обробники подій ==========
        private void btnLots_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функціонал для лотів у розробці", "Інформація",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e) => ShowUsersSection();

        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функціонал для категорій у розробці", "Інформація",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSecretCodes_Click(object sender, RoutedEventArgs e) => ShowSecretCodesSection();

        private void btnLogs_Click(object sender, RoutedEventArgs e) => ShowLogsSection();

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
                    MessageBox.Show(successMessage, "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    dgMainData.Items.Refresh();
                    if (_currentSection == AdminSection.Users)
                        UpdateStatsPanel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при видаленні: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var serviceProvider = App.ServiceProvider;
            var preUserApiClient = serviceProvider.GetRequiredService<PreUserApiClient>();

            var authWindow = new AuthorizationWindow(
                serviceProvider,
                preUserApiClient,
                user =>
                {
                    if (user.InterfaceType == EnumInterfaceTypeDto.Administrator)
                    {
                        var adminApiClient = serviceProvider.GetRequiredService<AdministratorApiClient>();
                        new AdminWindow(adminApiClient).Show();
                    }
                    // Додайте інші варіанти для інших ролей
                }
            );

            authWindow.Show();
            this.Close();
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