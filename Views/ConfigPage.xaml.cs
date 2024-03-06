using Microsoft.UI.Xaml.Controls;

using Winestro_A.ViewModels;

namespace Winestro_A.Views;

public sealed partial class ConfigPage : Page
{
    public Action<object, TextChangedEventArgs> TextChanged { get; set; } = ConfigValueChanged;
    
    public ConfigViewModel ViewModel
    {
        get;
    }

    public ConfigPage()
    {
        ViewModel = App.GetService<ConfigViewModel>();
        InitializeComponent();
    }

    private static void ConfigValueChanged(object sender, TextChangedEventArgs e)
    {
        
    }
}
