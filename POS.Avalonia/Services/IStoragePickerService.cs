using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace POS.Avalonia.Services;

public interface IStoragePickerService
{
    Task<string?> PickSaveFileAsync(string suggestedFileName, string extensionWithDot, CancellationToken cancellationToken = default);
    Task<Stream?> PickOpenFileAsync(string extensionWithDot, CancellationToken cancellationToken = default);
}
