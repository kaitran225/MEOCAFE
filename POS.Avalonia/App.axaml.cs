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
            var mainVm = Program.GetService<MainViewModel>();
            var mainWindow = new MainWindow { DataContext = mainVm };
            try
            {
                var store = Program.GetService<Services.ISettingsStore>();
                var sizeKey = store.Get("UI:Size") ?? Services.UiSizePresets.Key1080;
                Services.UiSizePresetValues.ApplyToApplication(sizeKey);
                var (w, h) = Services.UiSizePresets.GetDimensions(sizeKey);
                mainWindow.Width = w;
                mainWindow.Height = h;
            }
            catch { /* first run: keep XAML default size */ }
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