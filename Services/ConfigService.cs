using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Winestro_A.Controls;

namespace Winestro_A.Services;

public class ConfigService
{
    public static ObservableCollection<KeyValuePairEditableR> EditableControls { get; private set; } = new();

    public static void Init()
    {
        foreach (var key in ApplicationData.Current.LocalSettings.Values.Keys)
        {
            LogService.Log(key);
            EditableControls.Add(new KeyValuePairEditableR()
            {
                Left = key,
                Right = ApplicationData.Current.LocalSettings.Values[key].ToString()
            });
        }
    }

    public static void CreateSetting(string key, string value)
    {
        ApplicationData.Current.LocalSettings.Values.Add(key, value);
    }

    public static bool RemoveSetting(string key)
    {
        return ApplicationData.Current.LocalSettings.Values.Remove(key);
    }
}
