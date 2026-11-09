using Avalonia;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using POS.Avalonia.Localization;
using POS.Core.Contracts;
using POS.Data;
using POS.Data.Repositories;
using System;
using System.Globalization;

namespace POS.Avalonia;

sealed class Program
{
    public static IHost? Host { get; private set; }
    public static string? DatabaseStartupError { get; private set; }

    public static event Action<int, int>? OnApplyDisplaySize;
    public static event Action? OnApplyFullScreen;

    public static void ApplyDisplaySize(int width, int height) => OnApplyDisplaySize?.Invoke(width, height);
    public static void ApplyFullScreen() => OnApplyFullScreen?.Invoke();

    [STAThread]
    public static void Main(string[] args)
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("vi-VN");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("vi-VN");

        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
                config.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLocalization();
                services.AddSingleton<IConfig, Services.ConfigFromConfiguration>();
                services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
                services.AddSingleton<Services.IDatabaseSyncService, Services.DatabaseSyncService>();
                services.AddSingleton<ICategoryRepository, CategoryRepository>();
                services.AddSingleton<IMenuItemRepository, MenuItemRepository>();
                services.AddSingleton<IRecipeRepository, RecipeRepository>();
                services.AddSingleton<IOrderRepository, OrderRepository>();
                services.AddSingleton<IOrderDetailRepository, OrderDetailRepository>();
                services.AddSingleton<ICustomerRepository, CustomerRepository>();
                services.AddSingleton<IDiscountRepository, DiscountRepository>();
                services.AddSingleton<IComboRepository, ComboRepository>();
                services.AddSingleton<IProductOptionRepository, ProductOptionRepository>();
                services.AddSingleton<IOrderDetailOptionRepository, OrderDetailOptionRepository>();
                services.AddSingleton<IShiftRepository, ShiftRepository>();
                services.AddSingleton<IShiftSessionRepository, ShiftSessionRepository>();
                services.AddSingleton<IAuditLogRepository, AuditLogRepository>();
                services.AddSingleton<ITableRepository, TableRepository>();
                services.AddSingleton<IRefundRepository, RefundRepository>();
                services.AddSingleton<IWasteRepository, WasteRepository>();
                services.AddSingleton<IProblemReportRepository, ProblemReportRepository>();
                services.AddSingleton<IAnalyticsRepository, AnalyticsRepository>();
                services.AddSingleton<Services.IReceiptService, Services.ReceiptService>();
                services.AddSingleton<Services.IPrintService, Services.ReceiptPrintService>();

                services.AddSingleton<Services.IAuditLogger, Services.AuditLogger>();
                services.AddSingleton<Services.CurrentUserService>();
                services.AddSingleton<Services.IAppNavigator, Services.AppNavigator>();
                services.AddSingleton<Services.ISettingsStore, Services.SettingsStore>();
                services.AddSingleton<Services.AppWindowDebugInfo>();
                services.AddSingleton<Services.IStoragePickerService, Services.AvaloniaStoragePickerService>();
                services.AddSingleton<Services.PendingTableService>();
                services.AddSingleton<Services.IViewModelResolver, Services.ViewModelCache>();
                services.AddSingleton<ViewModels.BentoHomeViewModel>();
                services.AddSingleton<ViewModels.ModuleHostViewModel>();
                services.AddSingleton<ViewModels.MainViewModel>();
                services.AddTransient<ViewModels.DashboardViewModel>();
                services.AddTransient<ViewModels.TableMapViewModel>();
                services.AddTransient<ViewModels.OrderSelectionViewModel>();
                services.AddTransient<ViewModels.RecipeBookViewModel>();
                services.AddTransient<ViewModels.MenuManagementViewModel>();
                services.AddTransient<ViewModels.InventoryViewModel>();
                services.AddTransient<ViewModels.DiscountManagementViewModel>();
                services.AddTransient<ViewModels.ShiftManagementViewModel>();
                services.AddTransient<ViewModels.OrderHistoryViewModel>();
                services.AddTransient<ViewModels.ShiftRegisterViewModel>();
                services.AddTransient<ViewModels.AuditLogViewModel>();
                services.AddTransient<ViewModels.ReportsViewModel>();
                services.AddTransient<ViewModels.DailySalesViewModel>();
                services.AddTransient<ViewModels.CostManagementViewModel>();
                services.AddTransient<ViewModels.StyleGuideViewModel>();
                services.AddTransient<ViewModels.WasteLogViewModel>();
                services.AddTransient<ViewModels.ProblemReportsViewModel>();
                services.AddTransient<ViewModels.SettingsViewModel>();
                services.AddTransient<ViewModels.BankTransactionViewModel>();
                services.AddTransient<ViewModels.MusicPlayerViewModel>();
                services.AddTransient<ViewModels.AiChatViewModel>();
            })
            .Build();

        var config = Host.Services.GetRequiredService<IConfiguration>();
        var logger = Host.Services.GetRequiredService<ILogger<Program>>();
        try
        {
            var dbPath = SqliteDatabaseEnsurer.EnsureExists(config);
            var builder = new SqliteConnectionStringBuilder { DataSource = dbPath };
            DatabaseMigrationRunner.Run(builder.ConnectionString);
            Host.Services.GetRequiredService<Services.CurrentUserService>().ApplyStaffDisplayName(config, Host.Services.GetRequiredService<Services.ISettingsStore>());
        }
        catch (Exception ex)
        {
            DatabaseStartupError = ex.Message;
            logger.LogError(ex, "Database initialization or migration failed.");
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    public static T GetService<T>() where T : class
    {
        if (Host?.Services.GetService(typeof(T)) is not T service)
            throw new InvalidOperationException($"{typeof(T).Name} is not registered.");
        return service;
    }

    public static IStringLocalizer GetLocalizer() =>
        Host!.Services.GetRequiredService<IStringLocalizer<StringsResource>>();
}
