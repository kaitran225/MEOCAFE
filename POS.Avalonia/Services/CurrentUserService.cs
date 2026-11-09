using Microsoft.Extensions.Configuration;
using POS.Core.Models;

namespace POS.Avalonia.Services;

public sealed class CurrentUserService
{
  private Employee? _current;

    public Employee? Current => _current;
    public bool IsLoggedIn => _current != null;

    public void ApplyStaffDisplayName(IConfiguration configuration, ISettingsStore store)
    {
        var name = store.Get("Staff:DisplayName")
                   ?? configuration["Staff:DisplayName"]
                   ?? "HienXanh";
        _current = new Employee
        {
            Id = 0,
            Fullname = name,
            Username = name,
            Role = "Staff"
        };
    }

    public void RefreshDisplayName(ISettingsStore store, IConfiguration configuration)
    {
        ApplyStaffDisplayName(configuration, store);
    }
}
