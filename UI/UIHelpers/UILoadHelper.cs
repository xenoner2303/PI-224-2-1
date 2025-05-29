using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Presentation.UIHelpers;

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

    public static BitmapImage ConvertBase64ToImage(string base64)
    {
        byte[] binaryData = Convert.FromBase64String(base64);

        using (var stream = new MemoryStream(binaryData))
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze(); // щоб уникнути проблем з потоками

            return image;
        }
    }
}
