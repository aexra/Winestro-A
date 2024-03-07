using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Winestro_A.Controls;
using Winestro_A.Services;
using Winestro_A.ViewModels;

namespace Winestro_A.Views;

public sealed partial class GuildsPage : Page
{
    public ObservableCollection<GuildButton> GuildsButtons
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
                ImageSource = guild.IconUrl
            }); ;
        }
    }
}
