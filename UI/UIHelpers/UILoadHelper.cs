using DTOsLibrary;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UI.UIHelpers;

internal static class UILoadHelper
{
    internal static void LoadEntities<TModel>(List<TModel> modelList, ComboBox comboBox, string displayMemberPath = null)
    {
        if (modelList != null && modelList.Count > 0 && comboBox != null)
        {
            comboBox.ItemsSource = modelList;
            comboBox.IsEnabled = true;

            if (!string.IsNullOrEmpty(displayMemberPath))
            {
                comboBox.DisplayMemberPath = displayMemberPath;
            }
        }
        else
        {
            comboBox.ItemsSource = null;
            comboBox.Items.Clear();
            comboBox.IsEnabled = false;
        }
    }

    internal static void LoadEntities<TModel>(List<TModel> modelList, ListBox listBox, ComboBox comboBox = null, string displayMemberPath = null)
    {
        if (modelList != null && modelList.Count > 0)
        {
            listBox.ItemsSource = modelList;
            listBox.IsEnabled = true;

            if (comboBox != null)
            {
                comboBox.ItemsSource = modelList;
                comboBox.IsEnabled = true;

                if (!string.IsNullOrEmpty(displayMemberPath))
                {
                    comboBox.DisplayMemberPath = displayMemberPath;
                }
            }
        }
        else
        {
            listBox.ItemsSource = new[] { "Дані відсутні" };
            listBox.IsEnabled = false;

            if (comboBox != null)
            {
                comboBox.ItemsSource = null;
                comboBox.Items.Clear();
                comboBox.IsEnabled = false;
            }
        }
    }

    public static BitmapImage LoadImage(AuctionLotDto auctionLotDto)
    {
        if (auctionLotDto.Image != null && auctionLotDto.Image.Bytes.Length > 0)
        {
            try
            {
                // створюємо потік із масиву байтів зображення
                using (var stream = new MemoryStream(auctionLotDto.Image.Bytes))
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

                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження зображення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        return null;
    }
}
