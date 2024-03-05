using static Winestro_A.Services.LogService;
using Microsoft.UI.Xaml.Controls;
using Winestro_A.ViewModels;
using Windows.System;
using Winestro_A.Services;

namespace Winestro_A.Views;

public sealed partial class MaestroPage : Page
{
    public string ConnectionStatus { get; set; }
    public string GuildsConnected { get; set; }
    public string PlayersActive { get; set; }
    public string CurrentTime { get; set; }
    public string RunTime { get; set; }

    public MaestroViewModel ViewModel
    {
        get;
    }

    public MaestroPage()
    {
        ViewModel = App.GetService<MaestroViewModel>();
        InitializeComponent();
    }

    private void RunBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        
    }

    private void CommandInput_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            IntegratedConsoleService.TryRun(((TextBox)sender).Text, out _);
            ((TextBox)sender).Text = string.Empty;
        }
    }
}
