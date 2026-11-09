using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using POS.Avalonia.ViewModels;
using POS.Avalonia.Views;

namespace POS.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            if (!string.IsNullOrEmpty(Program.DatabaseStartupError))
            {
                desktop.MainWindow = new MainWindow
                {
                    Content = new global::Avalonia.Controls.TextBlock
                    {
                        Text = "Không khởi tạo được cơ sở dữ liệu:\n" + Program.DatabaseStartupError,
                        Margin = new global::Avalonia.Thickness(24),
                        TextWrapping = global::Avalonia.Media.TextWrapping.Wrap
                    }
                };
                base.OnFrameworkInitializationCompleted();
                return;
            }
            var mainVm = Program.GetService<MainViewModel>();
            var mainWindow = new MainWindow { DataContext = mainVm };
            try
            {
                var store = Program.GetService<Services.ISettingsStore>();
                Services.ThemeApplier.Apply(store.Get("UI:Theme"));
            }
            catch { /* first run */ }
            try
            {
                var store = Program.GetService<Services.ISettingsStore>();
                var sizeKey = store.Get("UI:Size") ?? Services.UiSizePresets.Key1080;
                Services.UiSizePresetValues.ApplyToApplication(sizeKey);
                if (Services.UiSizePresets.IsFullScreen(sizeKey))
                    mainWindow.WindowState = global::Avalonia.Controls.WindowState.FullScreen;
                else
                {
                    var (w, h) = Services.UiSizePresets.GetDimensions(sizeKey);
                    mainWindow.Width = w;
                    mainWindow.Height = h;
                }
            }
            catch { /* first run */ }
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}