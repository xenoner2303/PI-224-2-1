using DTOsLibrary;
using Presentation.UIHelpers;
using System.IO;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace UI
{
    /// <summary>
    /// Interaction logic for LotCreationWindow.xaml
    /// </summary>
    public partial class LotCreationWindow : Window
    {
        private BaseUserDto lotOwner;
        public AuctionLotDto? CreatedLot { get; private set; }

        public LotCreationWindow(BaseUserDto lotOwner, List<CategoryDto> categories)
        {
            ArgumentNullException.ThrowIfNull(lotOwner, nameof(lotOwner));
            ArgumentNullException.ThrowIfNull(categories, nameof(categories));

            InitializeComponent();
            this.lotOwner = lotOwner;

            UILoadHelper.LoadEntities(categories, CategoryComboBox, "Name");
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

            if (CategoryComboBox.SelectedItem is not CategoryDto selectedCategory)
            {
                MessageBox.Show("Оберіть категорію");
                return;
            }

            byte[]? imageBytes = null;
            string imagePath = ImageUrlTextBox.Text.Trim();
            var image = new ImageDto();

            if (File.Exists(imagePath))
            {
                try
                {
                    imageBytes = File.ReadAllBytes(imagePath);
                    var imageType = Path.GetExtension(imagePath).ToLowerInvariant();

                    image.Bytes = imageBytes;
                    image.ContentType = imageType;
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
                Image = image,
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
