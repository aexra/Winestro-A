using Microsoft.UI.Xaml.Controls;

using Winestro_A.ViewModels;

namespace Winestro_A.Views;

public sealed partial class DebugPage : Page
{
    public DebugViewModel ViewModel
    {
        get;
    }

    public DebugPage()
    {
        ViewModel = App.GetService<DebugViewModel>();
        InitializeComponent();
    }
}
