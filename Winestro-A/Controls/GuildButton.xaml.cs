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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Winestro_A.Controls;

public sealed partial class GuildButton : UserControl
{
    public string ImageSource
    {
        get; set;
    } = "ms-appx:///Assets/LogMessageCommandIcon.png";
    public Button Button => guildButton;

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
}
