using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Winestro_A.Discord;
using Winestro_A.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Winestro_A.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MiscModulePage : Page
{
    public MiscModuleViewModel ViewModel
    {
        get;
    }

    public MiscModulePage()
    {
        ViewModel = App.GetService<MiscModuleViewModel>();
        InitializeComponent();
    }

    private async void SyncTest_Click(object sender, RoutedEventArgs e)
    {
        await DiscordBotService.TryRegisterAllCommandsToGuildAsync();
    }

    private async void SyncGlobal_Click(object sender, RoutedEventArgs e)
    {
        await DiscordBotService.TryRegisterAllCommandsGloballyAsync();
    }

    private async void UnsyncTest_Click(object sender, RoutedEventArgs e)
    {
        await DiscordBotService.TryUnregisterAllCommandsFromGuildAsync();
    }

    private async void UnsyncGlobal_Click(object sender, RoutedEventArgs e)
    {
        await DiscordBotService.TryUnregisterAllCommandsGloballyAsync();
    }
}
