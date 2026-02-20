using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

using POS.Activation;
using POS.Contracts.Services;
using POS.Core.Contracts.Services;
using POS.Core.Services;
using POS.Services;
using POS.Services.Dao;
using POS.ViewModels;
using POS.ViewModels.EmployeeViewModels;
using POS.ViewModels.Manager;
using POS.Views;
using POS.Views.EmployeeViews;
using POS.Views.Manager;

namespace POS;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar { get; set; }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDao, PostgreSqlDao>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<PickerService>();
            services.AddSingleton<ProductImportService>();
            services.AddSingleton<MlModelService>();
            //services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<MainPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<BlankPage>();
            services.AddTransient<BlankViewModel>();
            services.AddTransient<LoginPage>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<SettingsViewModel>();

            services.AddTransient<MenuManagementPage>();
            services.AddTransient<MenuManagementViewModel>();
            services.AddTransient<DiscountManagementPage>();
            services.AddTransient<DiscountManagementViewModel>();
            services.AddTransient<EmployeeManagementPage>();
            services.AddTransient<EmployeeManagementViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<Dashboard>();
            services.AddTransient<OrderSelectionPage>();
            services.AddTransient<OrderSelectionViewModel>();
            services.AddTransient<OrderHistoryPage>();
            services.AddTransient<OrderHistoryViewModel>();

            services.AddSingleton<RoleViewModel>();

            // Configuration
        }).
        Build();

        UnhandledException += App_UnhandledException;

        SetTheme();
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await App.GetService<IActivationService>().ActivateAsync(args);
    }

    private void SetTheme()
    {
        //var settings = ApplicationData.Current.LocalSettings;

        //switch (settings.Values["Theme"].ToString())
        //{
        //    case "Light":
        //        this.RequestedTheme = ApplicationTheme.Light;
        //        break;
        //    case "Dark":
        //        this.RequestedTheme = ApplicationTheme.Dark;
        //        break;
        //    default:
        //        this.RequestedTheme = ApplicationTheme.Light;
        //        break;
        //}
    }

    public static void SetAppTheme()
    {
        //(App.Current as App)?.SetTheme();
    }
}
