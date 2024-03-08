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
using Winestro_A.Discord;
using Discord;
using System.Windows;

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

        DiscordBotService.OnReadyEventListener += () => {
            DispatcherQueue.TryEnqueue(() => {
                Microsoft.UI.Xaml.VisualStateManager.GoToState(this, "On", true);
            });
        };
        DiscordBotService.OnDisconnectedEventListener += () => {
            DispatcherQueue.TryEnqueue(() => {
                Microsoft.UI.Xaml.VisualStateManager.GoToState(this, "Off", true);
            });
        };
    }

    private async void RunBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await DiscordBotService.Toggle();
    }

    private void SyncUIData(Microsoft.UI.Dispatching.DispatcherQueueTimer sender, object obj)
    {
        Data.ConnectionState = DiscordBotService.ConnectionState.ToString();
        Data.GuildsConnected = DiscordBotService.Guilds.Count.ToString();

        switch (DiscordBotService.ConnectionState)
        {
            case ConnectionState.Connected:
                Data.IsRunButtonEnabled = true;
                Data.RunBtnText = "Stop";
                break;
            case ConnectionState.Disconnected:
                Data.IsRunButtonEnabled = true;
                Data.RunBtnText = "Run";
                break;
            case ConnectionState.Connecting:
                Data.IsRunButtonEnabled = false;
                Data.RunBtnText = "Connecting";
                break;
            case ConnectionState.Disconnecting:
                Data.IsRunButtonEnabled = false;
                Data.RunBtnText = "Disconnecting";
                break;
            default:
                Data.IsRunButtonEnabled = false;
                Data.RunBtnText = "Fix me";
                break;
        }

        if (LogService.UpdateRequired)
        {
            LogService.ForceUpdateControlsCollections();
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
