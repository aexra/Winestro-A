using Microsoft.UI.Xaml.Controls;

using Winestro_A.ViewModels;

namespace Winestro_A.Views;

public sealed partial class ModulesPage : Page
{
    public ModulesViewModel ViewModel
    {
        get;
    }

    public ModulesPage()
    {
        ViewModel = App.GetService<ModulesViewModel>();
        InitializeComponent();
    }
}
