using System.Threading;
using System.Threading.Tasks;

namespace POS.Avalonia.Services;

public interface ISettingsStore
{
    string? Get(string key);
    void Set(string key, string? value);
    Task SaveAsync(CancellationToken cancellationToken = default);
}
