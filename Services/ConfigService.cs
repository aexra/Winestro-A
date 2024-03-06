using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Windows.Foundation.Collections;
using Windows.Storage;
using Winestro_A.Controls;

namespace Winestro_A.Services;

public class ConfigService
{
    public static ObservableCollection<KeyValuePairEditableR> EditableControls { get; private set; } = new();
    private static IPropertySet Values = ApplicationData.Current.LocalSettings.Values;

    public static void Init()
    {
        Values.MapChanged += (s, e) => {
            EditableControls.Clear();
            FillControlsList();
        };

        FillControlsList();
    }

    private static void FillControlsList()
    {
        foreach (var key in Values.Keys)
        {
            EditableControls.Add(new KeyValuePairEditableR()
            {
                Left = key,
                Right = Values[key].ToString()
            });
        }
    }

    public static bool CreateSetting(string key, string value)
    {
        if (Values.Keys.Contains(key)) return false;

        Values.Add(key, value);
        return true;
    }

    public static bool RemoveSetting(string key)
    {
        return Values.Remove(key);
    }
}
