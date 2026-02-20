using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace POS.Avalonia.Services;

/// <summary>Reads from IConfiguration; saves editable keys to appsettings.Local.json.</summary>
public sealed class SettingsStore : ISettingsStore
{
    private readonly IConfiguration _config;
    private readonly string _localPath;
    private readonly Dictionary<string, string?> _overrides = new();

    public SettingsStore(IConfiguration config)
    {
        _config = config;
        _localPath = Path.Combine(AppContext.BaseDirectory, "appsettings.Local.json");
    }

    public string? Get(string key)
    {
        if (_overrides.TryGetValue(key, out var v)) return v;
        return _config[key];
    }

    public void Set(string key, string? value)
    {
        _overrides[key] = value;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        var dict = new Dictionary<string, object>();
        foreach (var kv in _overrides)
        {
            var parts = kv.Key.Split(':', 2, StringSplitOptions.None);
            if (parts.Length == 1)
                dict[kv.Key] = kv.Value ?? "";
            else
            {
                if (!dict.ContainsKey(parts[0]))
                    dict[parts[0]] = new Dictionary<string, object>();
                var sub = (Dictionary<string, object>)dict[parts[0]];
                sub[parts[1]] = kv.Value ?? "";
            }
        }
        await using var fs = new FileStream(_localPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await JsonSerializer.SerializeAsync(fs, dict, new JsonSerializerOptions { WriteIndented = true }, cancellationToken).ConfigureAwait(false);
    }
}
