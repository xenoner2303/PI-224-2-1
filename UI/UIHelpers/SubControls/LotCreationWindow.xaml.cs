using DTOsLibrary;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UI
{
    /// <summary>
    /// Interaction logic for LotCreationWindow.xaml
    /// </summary>
    public partial class LotCreationWindow : Window
    {
        private BaseUserDto lotOwner;
        public AuctionLotDto? CreatedLot { get; private set; }

        public LotCreationWindow(BaseUserDto lotOwner, List<CategoryDto> flatCategoryList)
        {
            ArgumentNullException.ThrowIfNull(lotOwner, nameof(lotOwner));
            ArgumentNullException.ThrowIfNull(flatCategoryList, nameof(flatCategoryList));

            InitializeComponent();

            this.lotOwner = lotOwner;
            CategoryTreeView.ItemsSource = flatCategoryList;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(StartPriceTextBox.Text.Trim(), out decimal startPrice))
            {
                MessageBox.Show("Некоректне значення для початкової ціни");
                return;
            }

            if (!int.TryParse(DurationDaysTextBox.Text.Trim(), out int durationDays))
            {
                MessageBox.Show("Некоректне значення для тривалості");
                return;
            }

            if (CategoryTreeView.SelectedItem is not CategoryDto selectedCategory)
            {
                MessageBox.Show("Оберіть категорію");
                return;
            }

            ImageDto? image = null;
            string imagePath = ImageUrlTextBox.Text.Trim();

            if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    var imageType = Path.GetExtension(imagePath).ToLowerInvariant();

                    image = new ImageDto
                    {
                        Bytes = imageBytes,
                        ContentType = imageType
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не вдалося прочитати зображення: {ex.Message}");
                    return;
                }
            }

            // створюємо новий об'єкт AuctionLotDto
            CreatedLot = new AuctionLotDto
            {
                Title = TitleTextBox.Text.Trim(),
                Description = DescriptionTextBox.Text.Trim(),
                StartPrice = startPrice,
                DurationDays = durationDays,
                Image = image, // буде null, якщо зображення не вказане або не знайдене
                Category = selectedCategory,
                Owner = lotOwner
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ImageUrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string path = ImageUrlTextBox.Text.Trim();

            // перевірка, чи файл існує
            if (File.Exists(path))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze(); // заморожуємо для роботи в UI-потоці
                    }

                    PreviewImage.Source = bitmap;
                }
                catch (Exception ex)
                {
                    PreviewImage.Source = null;
                    Console.WriteLine($"Не вдалося завантажити зображення: {ex.Message}");
                }
            }
            else
            {
                PreviewImage.Source = null;
            }
        }
    }
}
