using static Winestro_A.Services.LogService;
using Microsoft.UI.Xaml.Controls;
using Winestro_A.ViewModels;
using Windows.System;
using Winestro_A.Services;
using System.Windows.Threading;
using Windows.Foundation;
using System.Reactive.Linq;
using Winestro_A.Models;
using Winestro_A.Helpers;
using System.Windows.Navigation;

namespace Winestro_A.Views;

public sealed partial class MaestroPage : Page
{
    public MainPageDataContainer Data
    {
        get;
        set;
    } = new MainPageDataContainer();

    private Microsoft.UI.Dispatching.DispatcherQueueTimer UIDataSyncTimer;

    public MaestroViewModel ViewModel
    {
        get;
    }

    public MaestroPage()
    {
        ViewModel = App.GetService<MaestroViewModel>();
        InitializeComponent();

        UIDataSyncTimer = CreateTimer(0.1, SyncUIData, true);



        Data.RunBtnText = "Fix Me";
        Data.RunBtnColor = "#ffffff";
        Data.ConnectionState = DiscordBotService.ConnectionState.ToString();
        Data.GuildsConnected = "0";
        Data.PlayersActive = "0";
        Data.CurrentTime = TimeHelper.NowS();
        Data.RunTime = "0";
    }

    private async void RunBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await DiscordBotService.Toggle();
    }

    private void SyncUIData(Microsoft.UI.Dispatching.DispatcherQueueTimer sender, object obj)
    {
        Data.ConnectionState = DiscordBotService.ConnectionState.ToString();
        switch (DiscordBotService.ConnectionState)
        {
            case Discord.ConnectionState.Connected:
                Data.RunBtnText = "Stop";
                break;
            case Discord.ConnectionState.Disconnected:
                Data.RunBtnText = "Run";
                break;
            case Discord.ConnectionState.Connecting:
                Data.RunBtnText = "Connecting";
                break;
            case Discord.ConnectionState.Disconnecting:
                Data.RunBtnText = "Disconnecting";
                break;
            default:
                Data.RunBtnText = "Fix me";
                break;
        }
    }

    private void CommandInput_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter && ((TextBox)sender).Text.Length > 0)
        {
            var promt = ((TextBox)sender).Text;
            IntegratedConsoleService.СonsolePromts.Push(promt);
            IntegratedConsoleService.TryRun(promt, out _);
            ((TextBox)sender).Text = string.Empty;
        }
        if (e.Key == VirtualKey.Up && IntegratedConsoleService.СonsolePromts.Count() > 0)
        {
            ((TextBox)sender).Text = IntegratedConsoleService.СonsolePromts.Peek();
            ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
        }
    }

    private Microsoft.UI.Dispatching.DispatcherQueueTimer CreateTimer(double s, TypedEventHandler<Microsoft.UI.Dispatching.DispatcherQueueTimer, object> onTick, bool isRepeating = false, bool forceStart = true)
    {
        var timer = DispatcherQueue.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(s);
        timer.IsRepeating = isRepeating;
        timer.Tick += onTick;
        if (forceStart) timer.Start();
        return timer;
    }
}
