using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Controls;

using Winestro_A.Contracts.Services;
using Winestro_A.ViewModels;
using Winestro_A.Views;

namespace Winestro_A.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<MaestroViewModel, MaestroPage>();
        Configure<ModulesViewModel, ModulesPage>();
        Configure<MusicModuleViewModel, MusicModulePage>();
        Configure<MiscModuleViewModel, MiscModulePage>();
        Configure<ConfigViewModel, ConfigPage>();
        Configure<GuildsViewModel, GuildsPage>();
        Configure<DebugViewModel, DebugPage>();
        Configure<SettingsViewModel, SettingsPage>();
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    private void Configure<VM, V>()
        where VM : ObservableObject
        where V : Page
    {
        lock (_pages)
        {
            var key = typeof(VM).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(V);
            if (_pages.ContainsValue(type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}
