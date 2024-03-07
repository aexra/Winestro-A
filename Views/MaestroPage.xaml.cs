using static Winestro_A.Services.LogService;
using Microsoft.UI.Xaml.Controls;
using Winestro_A.ViewModels;
using Windows.System;
using Winestro_A.Services;
using System.Windows.Threading;
using Windows.Foundation;
using System.Reactive.Linq;

namespace Winestro_A.Views;

public sealed partial class MaestroPage : Page
{
    public string ConnectionState { get; set; }
    public string GuildsConnected   { get; set; }
    public string PlayersActive     { get; set; }
    public string CurrentTime       { get; set; }
    public string RunTime           { get; set; }

    public string RunBtnText { get; set; } = "Fix me";
    public string RunBtnColor { get; set; }

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
            ConnectionState = DiscordBotService.ConnectionState.ToString();
            switch (DiscordBotService.ConnectionState)
            {
                case Discord.ConnectionState.Connected:
                    RunBtnText = "Stop";
                    break;
                case Discord.ConnectionState.Disconnected:
                    RunBtnText = "Run";
                    break;
                case Discord.ConnectionState.Connecting:
                    RunBtnText = "Connecting";
                    break;
                case Discord.ConnectionState.Disconnecting:
                    RunBtnText = "Disconnecting";
                    break;
                default:
                    RunBtnText = "Fix me";
                    break;
            }
        });
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
