using DTOsLibrary;
using DTOsLibrary.DTOEnums;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Presentation.UIHelpers;
using Presentation.UIHelpers.SubControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        }

        private async void ManagerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var allCategories = await _client.GetAllCategoriesAsync();

                ManagerNameTextBlock.Text = _userDto?.Login ?? "Manager";

                await LoadCategoriesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження даних: " + ex.Message);
            }
        }

        private List<CategoryDto> BuildCategoryTree(List<CategoryDto> categories)
        {
            var lookup = categories.ToDictionary(c => c.Id);
            List<CategoryDto> rootCategories = new();

            foreach (var category in categories)
            {
                category.Subcategories = new(); // очищення перед побудовою

                if (category.ParentId == null)
                {
                    rootCategories.Add(category);
                }
                else if (lookup.TryGetValue(category.ParentId.Value, out var parent))
                {
                    parent.Subcategories.Add(category);
                }
            }

            return rootCategories;
        }
        private async Task CategoryTreeView_Load(object sender, RoutedEventArgs e)
        {
            try
            {
                var categories = await _client.GetAllCategoriesAsync();
                var tree = BuildCategoryTree(categories);
                CategoryTreeView.ItemsSource = tree;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}");
            }
        }
        private void CategoryTreeView_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            if (CategoryTreeView.SelectedItem is TreeViewItem selectedItem)
            {
                if (selectedItem.Tag is DTOsLibrary.CategoryDto category)
                {
                    selectedCategoryId = category.Id;  // припускаю, що Id — це int
                }
                else
                {
                    selectedCategoryId = null;
                }
            }
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
                    var tree = BuildCategoryTree(allCategories);
                    CategoryTreeView.ItemsSource = tree;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося оновити дерево категорій: " + ex.Message);
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

            var status = receiveLots[0].Status;

            switch (status)
            {
                case EnumLotStatusesDto.Pending:
                    PendingDataGrid.ItemsSource = receiveLots;
                    MainTabControl.SelectedIndex = 0;
                    break;
                case EnumLotStatusesDto.Active:
                    ActiveLotsDataGrid.ItemsSource = receiveLots;
                    MainTabControl.SelectedIndex = 1;
                    break;
                case EnumLotStatusesDto.Completed:
                    FinishedLotsDataGrid.ItemsSource = receiveLots;
                    MainTabControl.SelectedIndex = 2;
                    break;
                case EnumLotStatusesDto.Rejected:
                    RejectedLotsDataGrid.ItemsSource = receiveLots;
                    MainTabControl.SelectedIndex = 3;
                    break;
                default:
                    MessageBox.Show("Невідомий статус лотів.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

        }
        private void UpdateLotsTab()
        {
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
                        GetNeededLots(EnumLotStatusesDto.Pending);
                        break;
                    case "Активні торги":
                        GetNeededLots(EnumLotStatusesDto.Active);
                        break;
                    case "Завершені торги":
                        GetNeededLots(EnumLotStatusesDto.Completed);
                        break;
                    case "Відхилені лоти":
                        GetNeededLots(EnumLotStatusesDto.Rejected);
                        break;
                }
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
                    PendingDataGrid.ItemsSource = allLots.Where(lot => lot.Status == DTOsLibrary.DTOEnums.EnumLotStatusesDto.Pending).ToList();
                    break;
                case EnumLotStatusesDto.Active:
                    ActiveLotsDataGrid.ItemsSource = allLots.Where(lot => lot.Status == DTOsLibrary.DTOEnums.EnumLotStatusesDto.Active).ToList();
                    break;
                case EnumLotStatusesDto.Completed:
                    FinishedLotsDataGrid.ItemsSource = allLots.Where(lot => lot.Status == DTOsLibrary.DTOEnums.EnumLotStatusesDto.Completed).ToList();
                    break;
                case EnumLotStatusesDto.Rejected:
                    RejectedLotsDataGrid.ItemsSource = allLots.Where(lot => lot.Status == DTOsLibrary.DTOEnums.EnumLotStatusesDto.Rejected).ToList();
                    break;
                default:
                    return;
            }
        }
        private async void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTreeViewItem = CategoryTreeView.SelectedItem as TreeViewItem;
            var selectedCategory = selectedTreeViewItem?.Tag as CategoryDto;
            string name = NewCategoryTextBox.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введіть назву категорії.");
                return;
            }

            var newCategoryDto = new CategoryDto
            {
                Name = name,
                ParentId = selectedCategory?.Id
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
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem item && item.DataContext is CategoryDto category)
            {
                item.IsSelected = true;
                e.Handled = true;

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
                item.ContextMenu = contextMenu;
                contextMenu.IsOpen = true;
            }
        }
        private async Task DeleteSelectedCategory(CategoryDto categoryDto)
        {
            try
            {
                var allCategories = await _client.GetAllCategoriesAsync();
                if (categoryDto != null)
                {
                    // Знайти всі підкатегорії
                    List<CategoryDto>? childCategories = allCategories
                        .Where(c => c.ParentId != null && c.ParentId == categoryDto.Id)
                        .ToList();

                    // Рекурсивно видалити їх
                    foreach (var child in childCategories)
                    {
                        await DeleteSelectedCategory(child);
                    }

                    // Видалити саму категорію
                    allCategories.Remove(categoryDto);
                    await _client.DeleteCategoryAsync(categoryDto.Id);
                    UpdateCategoryTreeView();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var client = _serviceProvider.GetRequiredService<PreUserApiClient>();

            var authWindow = new AuthorizationWindow(_serviceProvider, client, user =>
            {
                _userDto = user;
            });

            authWindow.Owner = this;
            authWindow.ShowDialog();
        }
        private async Task LoadCategoriesAsync()
        {
            try
            {
                var allCategories = await _client.GetAllCategoriesAsync();
                var tree = BuildCategoryTree(allCategories);
                CategoryTreeView.ItemsSource = tree;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося завантажити категорії: " + ex.Message);
            }
        }


    }
}