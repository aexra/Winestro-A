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
using Microsoft.UI.Xaml.Media.Imaging;
using System.Windows;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Winestro_A.Controls;

public sealed partial class DiscordChannelMessageControl : UserControl
{
    public IMessage Message { get; set; }
    public string AvatarUrl { get; set; }
    public string DisplayName { get; set; }
    public string TimeStamp { get; set; }

    public DiscordChannelMessageControl(IMessage msg)
    {
        this.InitializeComponent();
        FillData(msg);
    }

    private void FillData(IMessage msg)
    {
        Message = msg;
        AvatarUrl = msg.Author.GetAvatarUrl();
        DisplayName = ((SocketGuildUser)msg.Author).DisplayName;
        var localtime = msg.Timestamp.ToLocalTime();
        TimeStamp = localtime.TimeOfDay.ToString()[..5] + ", " + localtime.Day.ToString() + "." + localtime.Month.ToString() + "." + localtime.Year.ToString();

        if (!string.IsNullOrWhiteSpace(msg.CleanContent))
        {
            var tb = new TextBlock();
            tb.Text = msg.CleanContent;

            ContentStackPanel.Children.Add(tb);
        }

        foreach (var att in msg.Attachments)
        {
            if (att.ContentType != null)
            {
                if (att.ContentType.Contains("image"))
                {
                    var img = new Microsoft.UI.Xaml.Controls.Image();
                    img.Source = new BitmapImage(new Uri(att.Url));
                    img.MaxHeight = 200;
                    img.MaxWidth = 500;
                    img.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Left;

                    ContentStackPanel.Children.Add(img);
                }
            }
        }
    }
}
