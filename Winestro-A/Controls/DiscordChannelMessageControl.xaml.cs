using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Discord;
using Discord.WebSocket;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Winestro_A.Controls;

public sealed partial class DiscordChannelMessageControl : UserControl
{
    public IMessage Message { get; set; }
    public string AvatarUrl { get; set; }
    public string DisplayName { get; set; }

    public DiscordChannelMessageControl(IMessage msg)
    {
        this.InitializeComponent();
        Message = msg;
        AvatarUrl = msg.Author.GetAvatarUrl();
        DisplayName = ((SocketGuildUser)msg.Author).DisplayName;
    }
}