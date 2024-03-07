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

namespace Winestro_A.Views;

public sealed partial class MaestroPage : Page
{
    public MainPageDataContainer Data
    {
        get;
        set;
    } = new MainPageDataContainer();

    private Microsoft.UI.Dispatching.DispatcherQueueTimer ConnectionStateLoop;

    public MaestroViewModel ViewModel
    {
        get;
    }

    public MaestroPage()
    {
        ViewModel = App.GetService<MaestroViewModel>();
        InitializeComponent();

        CreateTimer(0.1, (t, o) => { 
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
        });

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

    private void CreateTimer(double s, TypedEventHandler<Microsoft.UI.Dispatching.DispatcherQueueTimer, object> onTick, bool forceStart = true)
    {
        ConnectionStateLoop = DispatcherQueue.CreateTimer();
        ConnectionStateLoop.Interval = TimeSpan.FromSeconds(s);
        ConnectionStateLoop.IsRepeating = true;
        ConnectionStateLoop.Tick += onTick;
        if (forceStart) ConnectionStateLoop.Start();
    }
}
