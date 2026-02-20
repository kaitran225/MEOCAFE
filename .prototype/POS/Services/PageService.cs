using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using POS.Contracts.Services;
using POS.ViewModels;
using POS.ViewModels.Manager;
using POS.ViewModels.EmployeeViewModels;
using POS.Views;
using POS.Views.Manager;
using POS.Views.EmployeeViews;

namespace POS.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<MainViewModel, MainPage>();
        Configure<BlankViewModel, BlankPage>();
        Configure<LoginViewModel, LoginPage>();
        Configure<SettingsViewModel, SettingsPage>();
        Configure<MenuManagementViewModel, MenuManagementPage>();
        Configure<EmployeeManagementViewModel, EmployeeManagementPage>();
        Configure<OrderSelectionViewModel, OrderSelectionPage>();
        Configure<DiscountManagementViewModel, DiscountManagementPage>();
        Configure<DashboardViewModel, Dashboard>();
        Configure<OrderHistoryViewModel, OrderHistoryPage>();
        Configure<ShiftRegisterViewModel, ShiftRegister>();
        Configure<ShiftManagementViewModel, ShiftManagementPage>();


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
