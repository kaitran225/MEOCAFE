using Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using POS.Core.Contracts;
using POS.Data;
using POS.Data.Repositories;
using System;

namespace POS.Avalonia;

sealed class Program
{
    public static IHost? Host { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
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
                services.AddSingleton<IConfig, Services.ConfigFromConfiguration>();
                services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
                services.AddSingleton<ICategoryRepository, CategoryRepository>();
                services.AddSingleton<IMenuItemRepository, MenuItemRepository>();
                services.AddSingleton<IOrderRepository, OrderRepository>();
                services.AddSingleton<IOrderDetailRepository, OrderDetailRepository>();
                services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
                services.AddSingleton<ICustomerRepository, CustomerRepository>();
                services.AddSingleton<IDiscountRepository, DiscountRepository>();
                services.AddSingleton<IComboRepository, ComboRepository>();
                services.AddSingleton<IProductOptionRepository, ProductOptionRepository>();
                services.AddSingleton<IOrderDetailOptionRepository, OrderDetailOptionRepository>();
                services.AddSingleton<IShiftRepository, ShiftRepository>();
                services.AddSingleton<POS.Core.Contracts.IShiftSessionRepository, POS.Data.Repositories.ShiftSessionRepository>();
                services.AddSingleton<POS.Core.Contracts.IAuditLogRepository, POS.Data.Repositories.AuditLogRepository>();
                services.AddSingleton<POS.Core.Contracts.ITableRepository, POS.Data.Repositories.TableRepository>();
                services.AddSingleton<POS.Core.Contracts.IRefundRepository, POS.Data.Repositories.RefundRepository>();
                services.AddSingleton<IAnalyticsRepository, AnalyticsRepository>();
                services.AddSingleton<POS.Avalonia.Services.IReceiptService, POS.Avalonia.Services.ReceiptService>();
                services.AddSingleton<POS.Avalonia.Services.IPrintService, POS.Avalonia.Services.PrintServiceStub>();
                services.AddSingleton<POS.Avalonia.Services.IKitchenDisplayService, POS.Avalonia.Services.KitchenDisplayService>();
                services.AddSingleton<POS.Avalonia.Services.IAuditLogger, POS.Avalonia.Services.AuditLogger>();
                services.AddSingleton<Services.CurrentUserService>();
                services.AddSingleton<Services.IAppNavigator, Services.AppNavigator>();
                services.AddSingleton<POS.Avalonia.Services.ISettingsStore, POS.Avalonia.Services.SettingsStore>();
                services.AddSingleton<POS.Avalonia.Services.PendingTableService, POS.Avalonia.Services.PendingTableService>();
                services.AddSingleton<Services.IViewModelResolver, Services.ViewModelResolver>();
                services.AddTransient<ViewModels.MainViewModel>();
                services.AddTransient<ViewModels.ShellViewModel>();
                services.AddTransient<ViewModels.DashboardViewModel>();
                services.AddTransient<ViewModels.TableMapViewModel>();
                services.AddTransient<ViewModels.OrderSelectionViewModel>();
                services.AddTransient<ViewModels.MenuManagementViewModel>();
                services.AddTransient<ViewModels.InventoryViewModel>();
                services.AddTransient<ViewModels.CustomerManagementViewModel>();
                services.AddTransient<ViewModels.EmployeeManagementViewModel>();
                services.AddTransient<ViewModels.DiscountManagementViewModel>();
                services.AddTransient<ViewModels.ShiftManagementViewModel>();
                services.AddTransient<ViewModels.KitchenDisplayViewModel>();
                services.AddTransient<ViewModels.OrderHistoryViewModel>();
                services.AddTransient<ViewModels.ShiftRegisterViewModel>();
                services.AddTransient<ViewModels.AuditLogViewModel>();
                services.AddTransient<ViewModels.ReportsViewModel>();
                services.AddTransient<ViewModels.SettingsViewModel>();
            })
            .Build();

        var config = Host.Services.GetRequiredService<IConfiguration>();
        var connStr = $"Host={config["DB_HOST"] ?? "localhost"};Port={config["DB_PORT"] ?? "5432"};Username={config["DB_USER"] ?? "postgres"};Password={config["DB_PASSWORD"] ?? "123456"};Database={config["DB_NAME"] ?? "dev_meo_cf"}";
        try
        {
            DatabaseEnsurer.EnsureExists(connStr);
            DatabaseMigrationRunner.Run(connStr);
        }
        catch (Exception)
        {
            // Migrations may fail if DB not available; allow app to start for dev
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
}
