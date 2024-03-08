using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.UI.Xaml.Controls;
using Winestro_A.Controls;
using Winestro_A.Discord;
using Winestro_A.ViewModels;

namespace Winestro_A.Views;

public sealed partial class GuildsPage : Page
{
    public ObservableCollection<GuildButton> GuildsButtons
    {
        get; set;
    } = new();

    public ObservableCollection<GuildButton> ChannelsButtons
    {
        get; set;
    } = new();


    public GuildsViewModel ViewModel
    {
        get;
    }

    public GuildsPage()
    {
        ViewModel = App.GetService<GuildsViewModel>();
        InitializeComponent();

        FillGuildsButtons();
    }



    private void FillGuildsButtons()
    {
        foreach (var guild in DiscordBotService.Guilds)
        {
            GuildsButtons.Add(new GuildButton()
            {
                ImageSource = guild.IconUrl,
                Click = OnGuildButtonClick,
                GuildId = guild.Id,
            });
        }
    }

    private async Task OnGuildButtonClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await FillChannelsButtons(sender, e);
    }
    private async Task FillChannelsButtons(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var channels = await DiscordBotService.GetGuildTextChannels(((GuildButton)sender).GuildId);
        foreach (var channel in channels)
        {

        }
    }
}
