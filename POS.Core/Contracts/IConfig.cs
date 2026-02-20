namespace POS.Core.Contracts;

public interface IConfig
{
    string? GetSetting(string key);
}
