using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace POS.Avalonia.Services;

public sealed class AvaloniaStoragePickerService : IStoragePickerService
{
    public async Task<string?> PickSaveFileAsync(string suggestedFileName, string extensionWithDot, CancellationToken cancellationToken = default)
    {
        var provider = GetStorageProvider();
        if (provider == null) return null;

        var options = new FilePickerSaveOptions
        {
            SuggestedFileName = suggestedFileName,
            DefaultExtension = extensionWithDot,
            FileTypeChoices = new[] { new FilePickerFileType("CSV") { Patterns = new[] { "*" + extensionWithDot } } }
        };
        var file = await provider.SaveFilePickerAsync(options).WaitAsync(cancellationToken).ConfigureAwait(true);
        if (file?.TryGetLocalPath() is { } path)
            return path;
        return null;
    }

    public async Task<Stream?> PickOpenFileAsync(string extensionWithDot, CancellationToken cancellationToken = default)
    {
        var provider = GetStorageProvider();
        if (provider == null) return null;

        var options = new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("CSV") { Patterns = new[] { "*" + extensionWithDot } } }
        };
        var files = await provider.OpenFilePickerAsync(options).WaitAsync(cancellationToken).ConfigureAwait(true);
        if (files.Count == 0) return null;
        return await files[0].OpenReadAsync().WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    private static IStorageProvider? GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return null;
        return (desktop.MainWindow as TopLevel)?.StorageProvider;
    }
}
