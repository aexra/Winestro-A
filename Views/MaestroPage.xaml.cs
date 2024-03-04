using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Winestro_A.ViewModels;

namespace Winestro_A.Views;

public sealed partial class MaestroPage : Page
{
    public string ConnectionStatus { get; set; }
    public string GuildsConnected { get; set; }
    public string PlayersActive { get; set; }
    public string CurrentTime { get; set; }
    public string RunTime { get; set; }

    public MaestroViewModel ViewModel
    {
        get;
    }

    public MaestroPage()
    {
        ViewModel = App.GetService<MaestroViewModel>();
        InitializeComponent();
    }
}
