using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Winestro_A.Controls;

public sealed partial class KeyValuePairEditableR : UserControl
{
    public string Left
    {
        get; set;
    } = string.Empty;
    public string Right
    {
        get; set;
    } = string.Empty;
    public GridLength LeftWidth { get; set; } = new GridLength(400);
    public Action<object, TextChangedEventArgs>? TextChanged { get; set; }
    public Action<object, RoutedEventArgs>? OnXClick { get; set; }

    public KeyValuePairEditableR(string left, string right)
    {
        Left = left;
        Right = right;
        this.InitializeComponent();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextChanged?.Invoke(sender, e);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        OnXClick?.Invoke(sender, e);
    }
}
