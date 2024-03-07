using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Winestro_A.Controls;
public sealed partial class KeyValuePair : UserControl, INotifyPropertyChanged
{
    private string leftValue;
    private string rightValue;

    public string Left
    {
        get => leftValue;
        set
        {
            if (leftValue != value)
            {
                leftValue = value;
                NotifyPropertyChanged();
            }
        }
    }
    public string Right
    {
        get => rightValue;
        set
        {
            if (rightValue != value)
            {
                rightValue = value;
                NotifyPropertyChanged();
            }
        }
    }
    public GridLength LeftWidth { get; set; } = new GridLength(120);

    public KeyValuePair()
    {
        this.InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
