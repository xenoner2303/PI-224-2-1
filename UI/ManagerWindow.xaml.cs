using DTOsLibrary;
using DTOsLibrary.DTOEnums;
using Presentation.UIHelpers;
using Presentation.UIHelpers.SubControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UI.ApiClients;
namespace Presentation
{
    /// <summary>
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ManagerApiClient _client;
        private BaseUserDto _userDto;
        private List<AuctionLotDto> _allLots;
        private List<CategoryDto> _allCategories;
        private AuctionLotDto? _selectedLot;
        private int? selectedCategoryId = null;

        public ManagerWindow(IServiceProvider serviceProvider, ManagerApiClient client) : base()
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(client);
            InitializeComponent();
            _client = client;
            _serviceProvider = serviceProvider;

            Loaded += ManagerWindow_Loaded;
        }

        private async void ManagerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _allCategories = await _client.GetCategoriesAsync();
                _allLots = await _client.GetAuctionLotsAsync();

                ManagerNameTextBlock.Text = _userDto?.Login ?? "Manager"; // Якщо _userDto ще не ініціалізовано, перевірте це теж

                UpdateCategoryTreeView();
                // Можливо, ініціалізація інших даних
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження даних: " + ex.Message);
            }
        }
        private void CategoryTreeView_Load(object sender, RoutedEventArgs e)
        {
            UpdateCategoryTreeView();
        }
        private void CategoryTreeView_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            if (CategoryTreeView.SelectedItem is TreeViewItem selectedItem)
            {
                selectedCategoryId = (int?)selectedItem.Tag;
            }
        }
        private async Task Search()
        {
            string? keyword = SearchTextBox.Text.Trim().ToLower();

            SearchLotsDto search = new SearchLotsDto
            {
                Keyword = keyword,
                CategoryId = selectedCategoryId
            };

            var receiveLots = await _client.SearchLotsAsync(search);

            UpdateLotsTab(receiveLots);
        }
        private static DataGrid FindDataGridInTab(TabItem tab)
        {
            return FindChild<DataGrid>(tab);
        }
        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T tChild)
                    return tChild;

                var result = FindChild<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
        private void UpdateLotsTab(List<AuctionLotDto> receiveLots)
        {
            // Отримати обрану вкладку
            var selectedTab = MainTabControl.SelectedItem as TabItem;

            if (selectedTab == null)
                return;

            // Знайти DataGrid у вкладці
            var dataGrid = FindDataGridInTab(selectedTab);

            if (dataGrid == null)
                return;

            if (receiveLots.Count == 0 || receiveLots == null)
            {
                dataGrid.ItemsSource = null;

                // Заміна таблиці на повідомлення про відсутність лотів
                // Очистити parent і додати повідомлення
                var parent = dataGrid.Parent as Panel;
                if (parent != null)
                {
                    parent.Children.Clear();
                    parent.Children.Add(new TextBlock
                    {
                        Text = "Лоти не знайдено.",
                        Foreground = Brushes.Gray,
                        FontSize = 16,
                        Margin = new Thickness(10)
                    });
                }
            }
            else
            {
                // Якщо лоти є — відновити DataGrid
                var parent = dataGrid.Parent as Panel;
                if (parent != null && !parent.Children.Contains(dataGrid))
                {
                    parent.Children.Clear();
                    parent.Children.Add(dataGrid);
                }

                dataGrid.ItemsSource = receiveLots;
            }
        }
        private async void AcceptLot_Click(object sender, RoutedEventArgs e)
        {
            await _client.ApproveLotAsync(_selectedLot.Id);
        }
        private async void RejectLot_Click(object sender, RoutedEventArgs e)
        {
            await _client.RejectLotAsync(_selectedLot.Id);
        }
        private async void StopLot_Click(object sender, RoutedEventArgs e)
        {
            await _client.StopLotAsync(_selectedLot.Id);
        }
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await Search();
        }
        private void LotsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem is AuctionLotDto selectedLot)
            {
                _selectedLot = selectedLot;
                ShowLotInfo(_selectedLot);
            }
            else
            {
                _selectedLot = null;
            }
        }
        private void ShowLotInfo(AuctionLotDto selectedLot)
        {
            // Заповнення загальної інформації про лот
            LotDescriptionTextBlock.Text = selectedLot.Description;

            // Інформація про користувача, який створив лот
            UserFirstNameTextBlock.Text = selectedLot.Owner.FirstName;
            UserLastNameTextBlock.Text = selectedLot.Owner.LastName;
            UserPhoneNumberTextBlock.Text = selectedLot.Owner.PhoneNumber;

            AuctionLotImage.Source = UILoadHelper.LoadImage(selectedLot);

            if (selectedLot.Status == DTOsLibrary.DTOEnums.EnumLotStatusesDto.Completed && selectedLot.Bids.Count != 0)
            {
                var winner = selectedLot.Bids[selectedLot.Bids.Count - 1].User;
                // Показати блок переможця
                WinnerInfoPanel.Visibility = Visibility.Visible;

                // Заповнити інформацію про переможця
                WinnerFirstNameTextBlock.Text = winner.FirstName;
                WinnerLastNameTextBlock.Text = winner.LastName;
                WinnerPhoneNumberTextBlock.Text = winner.PhoneNumber;
            }
            else
            {
                // Сховати блок переможця
                WinnerInfoPanel.Visibility = Visibility.Collapsed;
            }
        }
        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem selectedTab)
            {
                string header = selectedTab.Header.ToString()!;

                switch (header)
                {
                    case "Непідтверджені лоти":
                        PendingDataGrid.ItemsSource = GetNeededLots(EnumLotStatusesDto.Pending);
                        break;
                    case "Активні торги":
                        ActiveLotsDataGrid.ItemsSource = GetNeededLots(EnumLotStatusesDto.Active);
                        break;
                    case "Завершені торги":
                        FinishedLotsDataGrid.ItemsSource = GetNeededLots(EnumLotStatusesDto.Completed);
                        break;
                    case "Відхилені лоти":
                        RejectedLotsDataGrid.ItemsSource = GetNeededLots(EnumLotStatusesDto.Rejected);
                        break;
                }
            }
        }
        private List<AuctionLotDto>? GetNeededLots(EnumLotStatusesDto enumLotStatus)
        {
            switch (enumLotStatus)
            {
                case EnumLotStatusesDto.Pending:
                    return _allLots.Where(lot => lot.Status == DTOsLibrary.DTOEnums.EnumLotStatusesDto.Pending).ToList();
                case EnumLotStatusesDto.Active:
                    return _allLots.Where(lot => lot.Status == DTOsLibrary.DTOEnums.EnumLotStatusesDto.Active).ToList();
                case EnumLotStatusesDto.Completed:
                    return _allLots.Where(lot => lot.Status == DTOsLibrary.DTOEnums.EnumLotStatusesDto.Completed).ToList();
                case EnumLotStatusesDto.Rejected:
                    return _allLots.Where(lot => lot.Status == DTOsLibrary.DTOEnums.EnumLotStatusesDto.Rejected).ToList();
                default:
                    return null;
            }
        }
        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            string newCategoryName = NewCategoryTextBox.Text;
            if (string.IsNullOrWhiteSpace(newCategoryName))
            {
                MessageBox.Show("Будь ласка, введіть назву категорії.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                var newCategory = new CategoryDto { Name = newCategoryName };
                _client.CreateCategoryAsync(newCategory.Name).Wait();
                _allCategories.Add(newCategory);
                UpdateCategoryTreeView();
                NewCategoryTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при додаванні категорії: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UpdateCategoryTreeView()
        {
            CategoryTreeView.Items.Clear();
            if(_allCategories == null || !_allCategories.Any())
            {
                return;
            }
            var rootCategories = _allCategories.Where(c => c.Parent.Id == null);
            foreach (var rootCategory in rootCategories)
            {
                var treeViewItem = CreateTreeViewItem(rootCategory);
                CategoryTreeView.Items.Add(treeViewItem);
            }
        }
        private TreeViewItem CreateTreeViewItem(CategoryDto category)
        {
            var item = new TreeViewItem
            {
                Header = category.Name,
                Tag = category.Id
            };

            var children = _allCategories.Where(c => c.Parent.Id == category.Id);
            foreach (var child in children)
            {
                item.Items.Add(CreateTreeViewItem(child));
            }

            return item;
        }
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.IsSelected = true; // Встановлюємо вибраний елемент
                e.Handled = true;       // Щоб подія не "йшла" далі
            }
            MenuItem menuItem = new MenuItem
            {
                Header = "Видалити"
            };
            menuItem.Click += (s, args) => DeleteSelectedCategory();
        }
        private void DeleteSelectedCategory()
        {
            if (CategoryTreeView.SelectedItem is TreeViewItem selectedItem &&
                selectedItem.Tag is int categoryId)
            {
                var categoryToRemove = _allCategories.FirstOrDefault(c => c.Id == categoryId);
                if (categoryToRemove != null)
                {
                    _allCategories.Remove(categoryToRemove);
                    _client.DeleteCategoryAsync(categoryId).Wait();
                    UpdateCategoryTreeView();
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть категорію для видалення.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
            //відкрити вікно авторизації
        }
    }
}
