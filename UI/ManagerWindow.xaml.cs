using DTOsLibrary;
using DTOsLibrary.DTOEnums;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using UI.UIHelpers;
using UI.ApiClients;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System;

namespace UI
{
    /// <summary>
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        private readonly ManagerApiClient _client;
        private BaseUserDto _userDto;
        private IServiceProvider _serviceProvider;
        private AuctionLotDto? _selectedLot;
        private int? selectedCategoryId = null;

        public ManagerWindow(BaseUserDto userDto, IServiceProvider serviceProvider, ManagerApiClient client) : base()
        {
            ArgumentNullException.ThrowIfNull(client);

            InitializeComponent();
            _serviceProvider = serviceProvider;
            _client = client;
            _userDto = userDto;
            Loaded += ManagerWindow_Loaded;
            DataContext = this;
            WinnerInfoPanel.Visibility = Visibility.Collapsed;
        }

        private async void ManagerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var allCategories = await _client.GetAllCategoriesAsync();

                ManagerNameTextBlock.Text = _userDto?.Login ?? "Manager";

                await UpdateCategoryTreeView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження даних: " + ex.Message);
            }
        }

        private void CategoryTreeView_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            if (CategoryTreeView.SelectedItem is CategoryDto category)
            {

                selectedCategoryId = category.Id;  // припускаю, що Id — це int

            }
            else
            {
                selectedCategoryId = null; // якщо нічого не вибрано, скидаємо Id
            }
        }

        //методи для того, щоб знмати видылення з елемента дерева
        private void CategoryTreeView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject clickedElement = e.OriginalSource as DependencyObject;

            // Перевірка: чи натиснуто не на TreeViewItem
            while (clickedElement != null && !(clickedElement is TreeViewItem))
            {
                clickedElement = VisualTreeHelper.GetParent(clickedElement);
            }

            if (clickedElement == null)
            {
                // Клік поза TreeViewItem — очищаємо вибір
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var selectedItem = CategoryTreeView.SelectedItem;
                    if (selectedItem != null)
                    {
                        var treeViewItem = GetSelectedTreeViewItem(CategoryTreeView);
                        if (treeViewItem != null)
                        {
                            treeViewItem.IsSelected = false;
                        }
                    }

                    selectedCategoryId = null;
                    // Якщо ти використовуєш привʼязку — онови Binding або ViewModel

                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }
        private TreeViewItem GetSelectedTreeViewItem(ItemsControl parent)
        {
            foreach (object item in parent.Items)
            {
                TreeViewItem childNode = parent.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                if (childNode == null)
                    continue;

                if (childNode.IsSelected)
                    return childNode;

                TreeViewItem result = GetSelectedTreeViewItem(childNode);
                if (result != null)
                    return result;
            }

            return null;
        }
        private async Task UpdateCategoryTreeView()
        {
            try
            {
                var allCategories = await _client.GetAllCategoriesAsync();

                if (allCategories == null || allCategories.Count == 0)
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
                    CategoryTreeView.ItemsSource = allCategories;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося оновити дерево категорій: " + ex.Message);
            }
        }
        private async Task DeleteSelectedCategory(CategoryDto categoryDto)
        {
            try
            {
                if (categoryDto != null)
                {
                    if (categoryDto.Subcategories.Count != 0 && categoryDto.Subcategories != null)
                    {
                        foreach (var child in categoryDto.Subcategories)
                        {
                            //рекурсивне видалення
                            await DeleteSelectedCategory(child);
                        }
                    }

                    await _client.DeleteCategoryAsync(categoryDto.Id);
                    UpdateCategoryTreeView();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategoryInTree = CategoryTreeView.SelectedItem as CategoryDto;

            string name = NewCategoryTextBox.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введіть назву категорії.");
                return;
            }

            if (name.Length < 4 || name.Length > 50)
            {
                MessageBox.Show("Назва категорії має бути від 4 до 50 символів.");
                return;
            }

            var newCategoryDto = new CategoryDto
            {
                Name = name,
                ParentId = selectedCategoryInTree?.Id
            };

            var createdCategory = await _client.CreateCategoryAsync(newCategoryDto);
            if (createdCategory != null)
            {
                await UpdateCategoryTreeView(); // тут await!
                NewCategoryTextBox.Clear();
                MessageBox.Show("Категорію успішно створено!");
            }
            else
            {
                MessageBox.Show("Не вдалося створити категорію.");
            }
        }
        private async void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            await _client.ApproveLotAsync(_selectedLot.Id);
            PendingDataGrid.SelectedItem = null;
            _selectedLot = null;
            ShowLotInfo(null); // Сховає всі кнопки та інформацію про лот      
            GetNeededLots(EnumLotStatusesDto.Pending); // Оновити список лотів
            AcceptButton.Visibility = Visibility.Collapsed; // Сховати кнопку після підтвердження
            RejectButton.Visibility = Visibility.Collapsed; // Сховати кнопку після підтвердження
        }
        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            DelitionReasonTextBox.Visibility = Visibility.Visible;
            ConfirmButton.Visibility = Visibility.Visible;
        }
        private async void RejectionButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedLot.RejectionReason = DelitionReasonTextBox.Text;
            await _client.RejectLotAsync(_selectedLot);
            PendingDataGrid.SelectedItem = null;
            _selectedLot = null;
            ShowLotInfo(null); // Сховає всі кнопки та інформацію про лот
            GetNeededLots(EnumLotStatusesDto.Pending); // Оновити список лотів
            AcceptButton.Visibility = Visibility.Collapsed; // Сховати кнопку після підтвердження
            RejectButton.Visibility = Visibility.Collapsed; // Сховати кнопку після підтвердження
            DelitionReasonTextBox.Visibility = Visibility.Collapsed; // Сховати поле для причини
            ConfirmButton.Visibility = Visibility.Collapsed; // Сховати кнопку підтвердження
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _client.StopLotAsync(_selectedLot.Id);
            PendingDataGrid.SelectedItem = null;
            _selectedLot = null;
            ShowLotInfo(null); // Сховає всі кнопки та інформацію про лот
            GetNeededLots(EnumLotStatusesDto.Active); // Оновити список лотів
            StopButton.Visibility = Visibility.Collapsed; // Сховати кнопку після зупинки
        }
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var findedLots = await SearchLots(
                SearchTextBox.Text,
                MainTabControl.SelectedItem as TabItem,
                CategoryTreeView.SelectedItem as CategoryDto
            );
            if (MainTabControl.SelectedItem is TabItem selectedTab)
            {
                string header = selectedTab.Header.ToString()!;

                switch (header)
                {
                    case "Непідтверджені лоти":
                        PendingDataGrid.ItemsSource = findedLots.Where(lot => lot.Status == EnumLotStatusesDto.Pending).ToList();
                        break;
                    case "Активні торги":
                        ActiveLotsDataGrid.ItemsSource = findedLots.Where(lot => lot.Status == EnumLotStatusesDto.Active).ToList();
                        break;
                    case "Закінчені торги":
                        FinishedLotsDataGrid.ItemsSource = findedLots.Where(lot => lot.Status == EnumLotStatusesDto.Completed).ToList();
                        break;
                    case "Відхилені лоти":
                        RejectedLotsDataGrid.ItemsSource = findedLots.Where(lot => lot.Status == EnumLotStatusesDto.Rejected).ToList();
                        break;
                }
            }
        }
        private void LotsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dg && dg.SelectedItem is AuctionLotDto lot)
            {
                _selectedLot = lot;
                ShowLotInfo(lot);

                e.Handled = true;   // ← зупиняємо бульбашіння
            }
        }
        private void ShowLotInfo(AuctionLotDto selectedLot)
        {
            if (selectedLot == null)
            {
                AcceptButton.Visibility = Visibility.Collapsed;
                RejectButton.Visibility = Visibility.Collapsed;
                DelitionReasonTextBox.Visibility = Visibility.Collapsed;
                ConfirmButton.Visibility = Visibility.Collapsed;
                StopButton.Visibility = Visibility.Collapsed;
                return;
            }

            LotDescriptionTextBlock.Text = selectedLot.Description;
            UserFirstNameTextBlock.Text = selectedLot.Owner.FirstName;
            UserLastNameTextBlock.Text = selectedLot.Owner.LastName;
            UserPhoneNumberTextBlock.Text = selectedLot.Owner.PhoneNumber;

            AuctionLotImage.Source = UILoadHelper.LoadImage(selectedLot);

            if (selectedLot.Status == EnumLotStatusesDto.Completed && selectedLot.Bids.Count > 0)
            {
                var winner = selectedLot.Bids[^1].User;
                WinnerInfoPanel.Visibility = Visibility.Visible;
                WinnerFirstNameTextBlock.Text = winner.FirstName;
                WinnerLastNameTextBlock.Text = winner.LastName;
                WinnerPhoneNumberTextBlock.Text = winner.PhoneNumber;
            }
            else
            {
                WinnerInfoPanel.Visibility = Visibility.Collapsed;
            }

            // Встановити видимість кнопок відповідно до статусу
            switch (selectedLot.Status)
            {
                case EnumLotStatusesDto.Pending:
                    AcceptButton.Visibility = Visibility.Visible;
                    RejectButton.Visibility = Visibility.Visible;
                    StopButton.Visibility = Visibility.Collapsed;
                    break;

                case EnumLotStatusesDto.Active:
                    AcceptButton.Visibility = Visibility.Collapsed;
                    RejectButton.Visibility = Visibility.Collapsed;
                    StopButton.Visibility = Visibility.Visible;
                    break;

                default:
                    AcceptButton.Visibility = Visibility.Collapsed;
                    RejectButton.Visibility = Visibility.Collapsed;
                    StopButton.Visibility = Visibility.Collapsed;
                    break;
            }

            // Завжди приховуємо поле для причини, поки не натиснуть "Відхилити"
            DelitionReasonTextBox.Visibility = Visibility.Collapsed;
            ConfirmButton.Visibility = Visibility.Collapsed;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem selectedTab)
            {
                string header = selectedTab.Header.ToString()!;

                switch (header)
                {
                    case "Непідтверджені лоти":
                        GetNeededLots(EnumLotStatusesDto.Pending);
                        break;
                    case "Активні торги":
                        GetNeededLots(EnumLotStatusesDto.Active);
                        break;
                    case "Закінчені торги":
                        GetNeededLots(EnumLotStatusesDto.Completed);
                        break;
                    case "Відхилені лоти":
                        GetNeededLots(EnumLotStatusesDto.Rejected);
                        break;
                }

                // Скидаємо попередній вибір
                _selectedLot = null;
                ShowLotInfo(null); // Сховає всі кнопки
            }
        }

        private async Task GetNeededLots(EnumLotStatusesDto enumLotStatus)
        {
            List<AuctionLotDto>? allLots = await _client.GetAuctionLotsAsync();
            if (allLots == null)
            {
                return;
            }

            switch (enumLotStatus)
            {
                case EnumLotStatusesDto.Pending:
                    PendingDataGrid.ItemsSource = allLots.Where(lot => lot.Status == EnumLotStatusesDto.Pending).ToList();
                    break;
                case EnumLotStatusesDto.Active:
                    ActiveLotsDataGrid.ItemsSource = allLots.Where(lot => lot.Status == EnumLotStatusesDto.Active).ToList();
                    break;
                case EnumLotStatusesDto.Completed:
                    FinishedLotsDataGrid.ItemsSource = allLots.Where(lot => lot.Status == EnumLotStatusesDto.Completed).ToList();
                    break;
                case EnumLotStatusesDto.Rejected:
                    RejectedLotsDataGrid.ItemsSource = allLots.Where(lot => lot.Status == EnumLotStatusesDto.Rejected).ToList();
                    break;
                default:
                    return;
            }
        }     

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Отримаємо елемент під курсором
            DependencyObject clickedElement = e.OriginalSource as DependencyObject;

            // Підіймаємось по візуальному дереву, доки не знайдемо TreeViewItem або не дійдемо до кореня
            while (clickedElement != null && !(clickedElement is TreeViewItem))
            {
                clickedElement = VisualTreeHelper.GetParent(clickedElement);
            }

            if (clickedElement is TreeViewItem clickedItem)
            {
                clickedItem.Focus();          // Наводимо фокус
                clickedItem.IsSelected = true; // Виділяємо цей елемент

                e.Handled = true;

                if (clickedItem.DataContext is CategoryDto category)
                {
                    var icon = new PackIcon
                    {
                        Kind = PackIconKind.Delete,
                        Width = 16,
                        Height = 16,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 2, 0)
                    };

                    MenuItem deleteItem = new MenuItem
                    {
                        Icon = icon
                    };

                    deleteItem.Click += async (s, args) => await DeleteSelectedCategory(category);

                    ContextMenu contextMenu = new ContextMenu();
                    contextMenu.Items.Add(deleteItem);
                    clickedItem.ContextMenu = contextMenu;
                    contextMenu.IsOpen = true;
                }
            }
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow? authWindow = null;

            try
            {
                var preUserApiClient = _serviceProvider.GetRequiredService<PreUserApiClient>();

                authWindow = new AuthorizationWindow(
                    _serviceProvider,
                    preUserApiClient,
                    user =>
                    {
                        if (user.InterfaceType == EnumInterfaceTypeDto.Manager)
                        {
                            this._userDto = user; // оновлюємо користувача
                            authWindow!.DialogResult = true; // закриває ShowDialog()
                        }
                    }
                );

                authWindow.Owner = this; // ставимо власником це вікно + щоб блокувалося якщо використовується ShowDialog()
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при кроку авторизації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            authWindow!.ShowDialog(); // визиваємо окремо, щоб не було зайвого обгортання та навантаження на програму
        }


        private async Task<List<AuctionLotDto>> SearchLots(string searchTitle, TabItem? searchStatus, CategoryDto searchCategory)
        {            
            var allLots = await _client.GetAuctionLotsAsync();
            if (string.IsNullOrEmpty(searchTitle))
            {
                return allLots;
            }
            // Отримуємо назву статусу з TabItem
            string? header = searchStatus?.Header?.ToString() ?? string.Empty;

            // Перетворюємо назву статусу в EnumLotStatusesDto, якщо можливо
            EnumLotStatusesDto? parsedStatus = null;
            if (!string.IsNullOrEmpty(header))
            {
                if (Enum.TryParse<EnumLotStatusesDto>(header, ignoreCase: true, out var status))
                {
                    parsedStatus = status;
                }
            }

            bool HasPartialMatch(string title, string search)
            {
                if (string.IsNullOrWhiteSpace(search) || string.IsNullOrWhiteSpace(title))
                    return false;

                title = title.ToLower();
                search = search.ToLower();

                if (search.Length < 3)
                {
                    // Якщо пошуковий рядок менший за 3 символи, шукаємо просто Contains
                    return title.Contains(search);
                }

                // Перевіряємо всі підрядки пошуку довжиною 3 і більше
                for (int length = 3; length <= search.Length; length++)
                {
                    for (int i = 0; i <= search.Length - length; i++)
                    {
                        var sub = search.Substring(i, length);
                        if (title.Contains(sub))
                            return true;
                    }
                }
                return false;
            }

            var filtered = allLots.Where(lot =>
                (string.IsNullOrEmpty(searchTitle) || HasPartialMatch(lot.Title, searchTitle))
                && (parsedStatus == null || lot.Status == parsedStatus)
                && (searchCategory == null || (lot.Category != null && lot.Category.Id == searchCategory.Id))
            ).ToList();

            return filtered;
        }




    }
}
