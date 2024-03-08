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
using Winestro_A.Services;
using Microsoft.UI.Xaml.Media.Animation;
using System.Reflection.Metadata;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Winestro_A.Controls;

public sealed partial class GuildButton : UserControl
{
    public ulong GuildId { get; set; }
    public string ImageSource
    {
        get; set;
    } = "ms-appx:///Assets/LogMessageCommandIcon.png";
    public Button Button => guildButton;

    public Func<object, RoutedEventArgs, Task> Click;

    public GuildButton()
    {
        this.InitializeComponent();
    }

    private void GuildButton_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "Hovered", true);
    }

    private void GuildButton_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "Normal", true);
    }

    private async void GuildButton_Click(object sender, RoutedEventArgs e)
    {
        if (Click != null) await Click(this, e);
    }
}
