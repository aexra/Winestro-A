using System.Collections.ObjectModel;
using System.Windows;
using Discord.Rest;
using Microsoft.UI.Xaml.Controls;
using Winestro_A.Controls;
using Winestro_A.Discord;
using Winestro_A.Services;
using Winestro_A.ViewModels;

namespace Winestro_A.Views;

public sealed partial class GuildsPage : Page
{
    public ulong? SelectedGuildId { get; set; }
    public ulong? SelectedChannelId { get; set; }

    public ObservableCollection<GuildButton> GuildsButtons { get; set; } = new();
    public ObservableCollection<ChannelButton> ChannelsButtons { get; set; } = new();
    public ObservableCollection<ChannelButton> MessagesControls { get; set; } = new();


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
        var guild = ((GuildButton)sender);
        if (guild.GuildId == SelectedGuildId)
        {
            await UpdateChannelsButtons(guild.GuildId);
            return;
        }

        SelectedGuildId = guild.GuildId;

        ChannelsButtons.Clear();
        GuildNameTB.Text = DiscordBotService.GetGuild(guild.GuildId).Name;

        var channels = await DiscordBotService.GetGuildTextChannelsAsync(guild.GuildId);
        foreach (var channel in channels)
        {
            ChannelsButtons.Add(new(channel.Name, channel.Id));
        }
    }
    private async Task UpdateChannelsButtons(ulong guild)
    {
        var channels = await DiscordBotService.GetGuildTextChannelsAsync(guild);
        List<ulong> channelIds = new();

        foreach (var control in ChannelsButtons)
        {
            channelIds.Add(control.ChannelId);
        }

        foreach (var channel in channels)
        {
            if (!channelIds.Contains(channel.Id))
                ChannelsButtons.Add(new(channel.Name, channel.Id));
        }
    }

    private async void ChannelsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var control = (ChannelButton)ChannelsLV.SelectedItem;
        if (control == null)
        {
            SelectedChannelId = null;
            await LoadSelectedChannelIfNotNull();
            return;
        }

        var channel = await DiscordBotService.GetTextChannelAsync(control.ChannelId);

        if (channel != null) 
        {
            SelectedChannelId = channel.Id;
            ChannelNameTB.Text = channel.Name;
        }
        else
        {
            SelectedChannelId = null;
            ChannelNameTB.Text = "[ERROR GETTING CHANNEL NAME]";
        }

        await LoadSelectedChannelIfNotNull();
    }
    private void EmptyChannelHistory()
    {
        MessagesControls.Clear();
    }
    private async Task FillChannelHistory(ulong channel)
    {
        
    }
    private async Task LoadSelectedChannelIfNotNull()
    {
        MessageBox.Text = string.Empty;
        if (SelectedChannelId == null)
        {
            EmptyChannelHistory();
            MessageBox.IsReadOnly = true;
            return;
        }
        else
        {
            MessageBox.IsReadOnly = false;
            await FillChannelHistory((ulong)SelectedChannelId);
        }
    }
}
