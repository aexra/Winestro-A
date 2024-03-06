using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Winestro_A.Services;
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

    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ContentDialog dialog = new ContentDialog();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = "New setting";
        dialog.PrimaryButtonText = "Create";
        dialog.CloseButtonText = "Cancel";
        dialog.DefaultButton = ContentDialogButton.Primary;
        //dialog.Content = new ContentDialogContent();

        var result = await dialog.ShowAsync();


        //if (!ConfigService.AddSetting("", "")) 
        //{
            
        //}
    }
}
