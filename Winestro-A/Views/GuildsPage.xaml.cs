using Microsoft.UI.Xaml.Controls;
using Winestro_A.Services;
using Winestro_A.ViewModels;

namespace Winestro_A.Views;

public sealed partial class GuildsPage : Page
{
    public GuildsViewModel ViewModel
    {
        get;
    }

    public GuildsPage()
    {
        ViewModel = App.GetService<GuildsViewModel>();
        InitializeComponent();
    }

    private void GuildButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var btn = (Button)sender;
        btn.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(15);
    }

    private void GuildButton_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var btn = (Button)sender;
        btn.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(30);
    }
}
