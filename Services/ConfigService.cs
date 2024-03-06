using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Microsoft.UI.Xaml.Controls;
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
            UpdateControlsList();
        };

        FillControlsList();
    }

    private static void UpdateControlsList()
    {
        try
        {
            foreach (var control in EditableControls)
            {
                if (!Values.Keys.Contains(control.Left))
                {
                    EditableControls.Remove(control);
                }
            }

            foreach (var key in Values.Keys)
            {
                var found = false;
                foreach (var control in EditableControls)
                {
                    if (control.Left == key)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    AddControl(key, Values[key].ToString());
                }
            }
        } catch (Exception ex) { }
    }

    private static void FillControlsList()
    {
        foreach (var key in Values.Keys)
        {
            AddControl(key, Values[key].ToString());
        }
    }

    private static void AddControl(string key, string value)
    {
        EditableControls.Add(new KeyValuePairEditableR(key, value)
        {
            TextChanged = (s, e) => {
                EditSetting(key, ((TextBox)s).Text);
            },
            OnXClick = (s, e) => {
                DeleteSetting(key);
            }
        });
    }

    public static bool AddSetting(string key, string value)
    {
        if (Values.Keys.Contains(key))
        {
            LogService.Error($"Failed creating setting [{key}={value}]");
            return false;
        }

        Values.Add(key, value);
        LogService.Log($"Created setting [{key}={value}]");
        return true;
    }

    public static bool DeleteSetting(string key)
    {
        var ok = Values.Remove(key);
        if (ok)
        {
            LogService.Log($"Removed setting [{key}]");
        }
        else
        {
            LogService.Error($"Failed removing setting [{key}]");
        }
        return ok;
    }

    public static void EditSetting(string key, string value)
    {
        Values[key] = value;
    }
}
