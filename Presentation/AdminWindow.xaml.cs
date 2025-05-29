using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using Presentation;

namespace OnlineAuction
{
    public partial class AdminWindow : Window
    {
        private List<AuctionLot> _lots = new List<AuctionLot>
        {
            new AuctionLot { Id = 1, Title = "Картина Ван Гога", CurrentBid = 1200.50m, EndDate = DateTime.Now.AddDays(3), Status = "Active" },
            new AuctionLot { Id = 2, Title = "Антикварна ваза", CurrentBid = 850.00m, EndDate = DateTime.Now.AddDays(1), Status = "Active" },
            new AuctionLot { Id = 3, Title = "Рідкісна монета", CurrentBid = 320.75m, EndDate = DateTime.Now.AddDays(-1), Status = "Completed" }
        };

        private List<User> _users = new List<User>
        {
            new User { Id = 1, Username = "admin", Email = "admin@auction.com", Role = "Administrator", RegistrationDate = DateTime.Now.AddMonths(-2) },
            new User { Id = 2, Username = "user1", Email = "user1@example.com", Role = "User", RegistrationDate = DateTime.Now.AddDays(-15) }
        };

        private List<SecretCode> _secretCodes = new List<SecretCode>
        {
            new SecretCode { Id = 1, Code = "GOLD2023", DiscountPercent = 15, ExpiryDate = DateTime.Now.AddMonths(1), IsActive = true },
            new SecretCode { Id = 2, Code = "SILVER50", DiscountPercent = 10, ExpiryDate = DateTime.Now.AddDays(15), IsActive = true },
            new SecretCode { Id = 3, Code = "EXPIRED", DiscountPercent = 20, ExpiryDate = DateTime.Now.AddDays(-1), IsActive = false }
        };

        public AdminWindow()
        {
            InitializeComponent();
            Loaded += AdminWindow_Loaded;
        }

        private void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShowLots();
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            btnLots.Click += (s, e) => ShowLots();
            btnUsers.Click += (s, e) => ShowUsers();
            btnCategories.Click += (s, e) => ShowCategories();
            btnSecretCodes.Click += (s, e) => ShowSecretCodes();
            btnReports.Click += (s, e) => ShowReports();

            btnAdd.Click += (s, e) => AddItem();
            btnEdit.Click += (s, e) => EditItem();
            btnDelete.Click += (s, e) => DeleteItem();
            btnLogout.Click += (s, e) => Logout();
        }

        #region Navigation Methods
        private void ShowLots()
        {
            tbCurrentSection.Text = "Лоти аукціону";
            dgMainData.ItemsSource = _lots;
            statsPanel.Visibility = Visibility.Visible;

            dgMainData.Columns.Clear();
            dgMainData.AutoGenerateColumns = false;

            dgMainData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id"), Width = DataGridLength.Auto });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Назва", Binding = new Binding("Title"), Width = DataGridLength.SizeToHeader });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Поточна ставка", Binding = new Binding("CurrentBid") { StringFormat = "C2" }, Width = DataGridLength.Auto });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Кінцева дата", Binding = new Binding("EndDate") { StringFormat = "dd.MM.yyyy HH:mm" }, Width = DataGridLength.Auto });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new Binding("Status"), Width = DataGridLength.Auto });
        }

        private void ShowUsers()
        {
            tbCurrentSection.Text = "Користувачі";
            statsPanel.Visibility = Visibility.Collapsed;
            dgMainData.ItemsSource = _users;

            dgMainData.Columns.Clear();
            dgMainData.AutoGenerateColumns = false;

            dgMainData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id"), Width = DataGridLength.Auto });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Логін", Binding = new Binding("Username"), Width = DataGridLength.SizeToHeader });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new Binding("Email"), Width = DataGridLength.SizeToHeader });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Роль", Binding = new Binding("Role"), Width = DataGridLength.Auto });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Дата реєстрації", Binding = new Binding("RegistrationDate") { StringFormat = "dd.MM.yyyy" }, Width = DataGridLength.Auto });
        }

        private void ShowSecretCodes()
        {
            tbCurrentSection.Text = "Секретні коди";
            statsPanel.Visibility = Visibility.Collapsed;
            dgMainData.ItemsSource = _secretCodes;

            dgMainData.Columns.Clear();
            dgMainData.AutoGenerateColumns = false;

            dgMainData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id"), Width = DataGridLength.Auto });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Код", Binding = new Binding("Code"), Width = DataGridLength.SizeToHeader });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Знижка (%)", Binding = new Binding("DiscountPercent"), Width = DataGridLength.Auto });
            dgMainData.Columns.Add(new DataGridTextColumn { Header = "Дійсний до", Binding = new Binding("ExpiryDate") { StringFormat = "dd.MM.yyyy" }, Width = DataGridLength.Auto });

            var statusColumn = new DataGridCheckBoxColumn
            {
                Header = "Активний",
                Binding = new Binding("IsActive"),
                Width = DataGridLength.Auto
            };
            dgMainData.Columns.Add(statusColumn);
        }

        private void ShowCategories()
        {
            tbCurrentSection.Text = "Категорії";
            statsPanel.Visibility = Visibility.Collapsed;
            // Тут буде логіка для категорій
            MessageBox.Show("Функціонал для категорій буде реалізовано пізніше", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowReports()
        {
            tbCurrentSection.Text = "Звіти";
            statsPanel.Visibility = Visibility.Visible;
            // Тут буде логіка для звітів
            MessageBox.Show("Функціонал для звітів буде реалізовано пізніше", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region CRUD Operations
        private void AddItem()
        {
            string currentSection = tbCurrentSection.Text;

            switch (currentSection)
            {
                case "Лоти аукціону":
                    var addLotWindow = new AddEditLotWindow();
                    if (addLotWindow.ShowDialog() == true)
                    {
                        _lots.Add(addLotWindow.Lot);
                        dgMainData.Items.Refresh();
                    }
                    break;

                case "Користувачі":
                    MessageBox.Show("Додавання нового користувача буде реалізовано пізніше", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;

                case "Секретні коди":
                    var addCodeWindow = new AddEditSecretCodeWindow();
                    if (addCodeWindow.ShowDialog() == true)
                    {
                        _secretCodes.Add(addCodeWindow.SecretCode);
                        dgMainData.Items.Refresh();
                    }
                    break;
            }
        }

        private void EditItem()
        {
            if (dgMainData.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, виберіть елемент для редагування", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string currentSection = tbCurrentSection.Text;

            switch (currentSection)
            {
                case "Лоти аукціону":
                    var selectedLot = dgMainData.SelectedItem as AuctionLot;
                    var editLotWindow = new AddEditLotWindow(selectedLot);
                    if (editLotWindow.ShowDialog() == true)
                    {
                        var index = _lots.FindIndex(l => l.Id == editLotWindow.Lot.Id);
                        if (index != -1)
                        {
                            _lots[index] = editLotWindow.Lot;
                            dgMainData.Items.Refresh();
                        }
                    }
                    break;

                case "Секретні коди":
                    var selectedCode = dgMainData.SelectedItem as SecretCode;
                    var editCodeWindow = new AddEditSecretCodeWindow(selectedCode);
                    if (editCodeWindow.ShowDialog() == true)
                    {
                        var index = _secretCodes.FindIndex(c => c.Id == editCodeWindow.SecretCode.Id);
                        if (index != -1)
                        {
                            _secretCodes[index] = editCodeWindow.SecretCode;
                            dgMainData.Items.Refresh();
                        }
                    }
                    break;
            }
        }

        private void DeleteItem()
        {
            if (dgMainData.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, виберіть елемент для видалення", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Ви впевнені, що хочете видалити обраний елемент?", "Підтвердження видалення",
                                      MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                string currentSection = tbCurrentSection.Text;

                switch (currentSection)
                {
                    case "Лоти аукціону":
                        var lotToRemove = dgMainData.SelectedItem as AuctionLot;
                        _lots.Remove(lotToRemove);
                        dgMainData.Items.Refresh();
                        break;

                    case "Секретні коди":
                        var codeToRemove = dgMainData.SelectedItem as SecretCode;
                        _secretCodes.Remove(codeToRemove);
                        dgMainData.Items.Refresh();
                        break;
                }
            }
        }
        #endregion

        private void Logout()
        {
            var result = MessageBox.Show("Ви дійсно хочете вийти з адмін-панелі?", "Підтвердження виходу",
                                      MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }

    #region Model Classes
    public class AuctionLot : INotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private decimal _currentBid;
        private DateTime _endDate;
        private string _status;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public decimal CurrentBid
        {
            get => _currentBid;
            set { _currentBid = value; OnPropertyChanged(nameof(CurrentBid)); }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(nameof(EndDate)); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class User : INotifyPropertyChanged
    {
        private int _id;
        private string _username;
        private string _email;
        private string _role;
        private DateTime _registrationDate;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        public string Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(nameof(Role)); }
        }

        public DateTime RegistrationDate
        {
            get => _registrationDate;
            set { _registrationDate = value; OnPropertyChanged(nameof(RegistrationDate)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SecretCode : INotifyPropertyChanged
    {
        private int _id;
        private string _code;
        private int _discountPercent;
        private DateTime _expiryDate;
        private bool _isActive;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Code
        {
            get => _code;
            set { _code = value; OnPropertyChanged(nameof(Code)); }
        }

        public int DiscountPercent
        {
            get => _discountPercent;
            set { _discountPercent = value; OnPropertyChanged(nameof(DiscountPercent)); }
        }

        public DateTime ExpiryDate
        {
            get => _expiryDate;
            set { _expiryDate = value; OnPropertyChanged(nameof(ExpiryDate)); }
        }

        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(nameof(IsActive)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion
}