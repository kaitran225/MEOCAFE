using Microsoft.Extensions.Configuration;
using POS.Core.Contracts;

namespace POS.Avalonia.Services;

public sealed class ConfigFromConfiguration : IConfig
{
    private readonly IConfiguration _configuration;
    public ConfigFromConfiguration(IConfiguration configuration) => _configuration = configuration;
    public string? GetSetting(string key) => _configuration[key];
}
