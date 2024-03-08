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

    public ObservableCollection<ChannelButton> ChannelsButtons
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
        GuildsButtons.Clear();
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
        ChannelsButtons.Clear();

        var guild = ((GuildButton)sender);
        GuildNameTB.Text = DiscordBotService.GetGuild(guild.GuildId).Name;

        var channels = await DiscordBotService.GetGuildTextChannelsAsync(guild.GuildId);
        foreach (var channel in channels)
        {
            ChannelsButtons.Add(new(channel.Name, channel.Id));
        }
    }
}
