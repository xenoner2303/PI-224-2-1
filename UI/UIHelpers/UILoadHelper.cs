using System.Windows.Controls;

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
}
