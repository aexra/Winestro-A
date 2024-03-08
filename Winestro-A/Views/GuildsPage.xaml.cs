using System.Collections.ObjectModel;
using System.Threading.Channels;
using System.Windows;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
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
    public ObservableCollection<DiscordChannelMessageControl> MessagesControls { get; set; } = new();


    public GuildsViewModel ViewModel
    {
        get;
    }

    public GuildsPage()
    {
        ViewModel = App.GetService<GuildsViewModel>();
        InitializeComponent();

        DiscordBotService.ChatOnMessageEventListener += OnMessageEvent;

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
            await DeselectTextChannel();
            return;
        }

        var channel = await DiscordBotService.GetTextChannelAsync(control.ChannelId);

        if (channel != null) 
        {
            if (channel.Id != SelectedChannelId)
            {
                SelectedChannelId = channel.Id;
                MessageBox.IsReadOnly = false;
                MessagesControls.Clear();
                await FillChannelHistory();
            }
            else
            {
                MessageBox.IsReadOnly = false;
                await UpdateChannelHistory();
            }
        }
        else
        {
            await DeselectTextChannel();
        }
    }
    private async Task FillChannelHistory(int limit = 100)
    {
        if (SelectedChannelId != null)
        {
            var ch = await DiscordBotService.GetTextChannelAsync((ulong)SelectedChannelId);
            if (ch is ITextChannel tch)
            {
                var msgs = tch.GetMessagesAsync(limit);

                var ccounter = 0;
                await foreach (var collection in msgs)
                {
                    foreach (var msg in collection)
                    {
                        MessagesControls.Insert(ccounter * limit, new(msg));
                    }
                    ccounter++;
                }
            }
        }
    }
    private async Task UpdateChannelHistory(int limit = 100)
    {
        List<ulong> hasmsg = new();

        foreach (var control in MessagesControls)
        {
            hasmsg.Add(control.Message.Id);
        }

        if (SelectedChannelId != null)
        {
            var ch = await DiscordBotService.GetTextChannelAsync((ulong)SelectedChannelId);
            if (ch is ITextChannel tch)
            {
                var msgs = tch.GetMessagesAsync(limit);

                await foreach (var collection in msgs)
                {
                    foreach (var msg in collection)
                    {
                        if (!hasmsg.Contains(msg.Id))
                        {
                            MessagesControls.Add(new(msg));
                        }
                            
                    }
                }
            }
        }
    }
    private async Task DeselectTextChannel()
    {
        MessagesControls.Clear();
        SelectedChannelId = null;
        MessageBox.IsReadOnly = true;
        ChannelNameTB.Text = string.Empty;
    }

    private void OnMessageEvent(SocketMessage msg)
    {
        if (msg.Channel.Id == SelectedChannelId)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                MessagesControls.Add(new(msg));
            });
        }
    }

    private async void MessageBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        var tb = (TextBox)sender;
        if (e.Key == Windows.System.VirtualKey.Enter && !string.IsNullOrWhiteSpace(tb.Text) && SelectedChannelId != null)
        {
            await DiscordBotService.SendTextMessageAsync((ulong)SelectedChannelId, tb.Text);
            tb.Text = string.Empty;
        }
    }
}
