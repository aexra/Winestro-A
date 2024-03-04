using Microsoft.UI.Xaml.Controls;

using Winestro_A.ViewModels;

namespace Winestro_A.Views;

public sealed partial class ConfigPage : Page
{
    public ConfigViewModel ViewModel
    {
        get;
    }

    public ConfigPage()
    {
        ViewModel = App.GetService<ConfigViewModel>();
        InitializeComponent();
    }
}
