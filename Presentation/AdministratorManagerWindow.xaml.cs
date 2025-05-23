using BLL.Commands;
using BLL.EntityBLLModels;
using Presentation.UIHelpers;
using System.Windows;
using System.Windows.Controls;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for AdministratorManagerWindow.xaml
    /// </summary>
    public partial class AdministratorManagerWindow : Window
    {
        private AdministratorCommandsManager administratorCommandManager;

        public AdministratorManagerWindow(AdministratorCommandsManager administratorCommandManager)
        {
            InitializeComponent();

            this.administratorCommandManager = administratorCommandManager ?? throw new ArgumentNullException(nameof(administratorCommandManager));
           
            LoadAdministratorWindowEntities();
        }

        private void ShowLogs_Click(object sender, RoutedEventArgs e)
        {
            var selectedDate = LogDatePicker.SelectedDate;

            try
            {
                var logs = administratorCommandManager.LoadLogs(selectedDate);
                if (logs != null && logs.Count > 0)
                {
                    LogListBox.ItemsSource = logs;
                }
                else
                {
                    LogListBox.ItemsSource = new[] { "Логи відсутні" };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при отриманні логів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var secretCodeRealizatorModel = new SecretCodeRealizatorModel
                {
                    SecretCode = NewSecretCodeTextBox.Text.Trim(),
                    CodeUses = int.TryParse(CodeUsesTextBox.Text, out var uses) ?
                               uses : throw new ArgumentException("Кількість використань повинна бути числом"),
                    RealizatorType = (BusinessEnumInterfaceType)AccountTypeComboBox.SelectedItem
                };

                if (administratorCommandManager.CreateCodeRealizator(secretCodeRealizatorModel))
                {
                    UILoadHelper.LoadEntities(administratorCommandManager.LoadSecretCodeRealizators(), CodesListBox);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при створенні реалізатору коду: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveCode_Click(object sender, RoutedEventArgs e)
        {
            var codeRealizator = CodesListBox.SelectedItem as SecretCodeRealizatorModel;

            if (codeRealizator == null)
            {
                return;
            }

            try
            {
                if (administratorCommandManager.RemoveSecretCodeRealizator(codeRealizator))
                {
                    UILoadHelper.LoadEntities(administratorCommandManager.LoadSecretCodeRealizators(), CodesListBox);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при видаленні реалізатора коду: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpgradeUsers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // отримуємо джерело даних дата сітки (додатково саме перевіряю що це саме користувачі)
                if (UsersDataGrid.ItemsSource is IEnumerable<BaseUserModel> usersEnumerable)
                {
                    var userList = usersEnumerable.ToList();

                    if (administratorCommandManager.SaveUsersChanges(userList))
                    {
                        // оновлюємо відображення користувачів після збереження
                        UsersDataGrid.ItemsSource = administratorCommandManager.LoadUsers();
                    }
                }
                else
                {
                    MessageBox.Show("Не вдалося отримати список користувачів або невірний формат даних", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні змін користувачів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = UsersDataGrid.SelectedItem as BaseUserModel;

            if(selectedUser == null)
            {
                return;
            }

            // додаткове вікно для перепитання щодо видалення користувача
            var result = MessageBox.Show(
                $"Ви дійсно бажаєте видалити користувача '{selectedUser.Login}' ?",
                "Підтвердження",
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                if (administratorCommandManager.RemoveUser(selectedUser))
                {
                    UsersDataGrid.ItemsSource = administratorCommandManager.LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при видаленні користувача: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAdministratorWindowEntities()
        {

            // налаштовуємо комбобокс для типів реалізаторів
            AccountTypeComboBox.ItemsSource = Enum.GetValues<BusinessEnumInterfaceType>()
                .Where(type => type != BusinessEnumInterfaceType.Registered)
                .ToList();

            // завантажуємо дані в комбобокс
            UILoadHelper.LoadEntities(administratorCommandManager.LoadSecretCodeRealizators(), CodesListBox);

            //налаштовуємо комбобокс дата сітки для типів користувачів
            var comboBoxColumn = UsersDataGrid.Columns
                                .OfType<DataGridComboBoxColumn>()
                                .FirstOrDefault(c => c.Header?.ToString() == "Роль");

            if (comboBoxColumn != null)
            {
                comboBoxColumn.ItemsSource = Enum.GetValues(typeof(BusinessEnumInterfaceType));
            }

            // завантажуємо дані в дата сітку
            UsersDataGrid.ItemsSource = administratorCommandManager.LoadUsers();
        }
    }
}
