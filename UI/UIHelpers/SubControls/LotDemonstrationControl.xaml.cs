using DTOsLibrary;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Presentation.UIHelpers.SubControls
{
    /// <summary>
    /// Interaction logic for LotDemonstrationControl.xaml
    /// </summary>
    public partial class LotDemonstrationControl : UserControl
    {
        public AuctionLotDto auctionLotDto;
        public event EventHandler? LotSelected;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                Background = isSelected ? Brushes.LightBlue : Brushes.Transparent;
            }
        }
        private bool isSelected = false;


        public LotDemonstrationControl(AuctionLotDto auctionLotDto)
        {
            InitializeComponent();

            this.auctionLotDto = auctionLotDto;
            FillControlParts();
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LotSelected?.Invoke(this, EventArgs.Empty);
        }

        private void FillControlParts()
        {
            if (auctionLotDto == null)
            {
                return;
            }

            LotNameTextBlock.Text = auctionLotDto.Title;
            PosterNameTextBlock.Text = auctionLotDto.Owner?.ToString() ?? "Невідомий";
            DescriptionTextBlock.Text = auctionLotDto.Description;

            if (auctionLotDto.ImageBytes != null && auctionLotDto.ImageBytes.Length > 0)
            {
                try
                {
                    // створюємо потік із масиву байтів зображення
                    using (var stream = new MemoryStream(auctionLotDto.ImageBytes))
                    {
                        // новий об’єкт BitmapImage для подальшого відображення
                        var bitmap = new BitmapImage();

                        // початок ініціалізації властивостей bitmap
                        bitmap.BeginInit();

                        // встановлюємо режим кешування - повне завантаження в пам’ять
                        // що дозволяє закрити потік одразу після завершення ініціалізації
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;

                        // встановлюємо джерело зображення – наш MemoryStream
                        bitmap.StreamSource = stream;

                        // завершуємо ініціалізацію bitmap
                        bitmap.EndInit();

                        // заморожуємо об’єкт bitmap, щоб зробити його потокобезпечним і незмінним
                        bitmap.Freeze();

                        LotImage.Source = bitmap;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка завантаження зображення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
