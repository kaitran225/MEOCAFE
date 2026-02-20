using System;
using POS.Core.Models;

namespace POS.Avalonia.Services;

public sealed class CurrentUserService
{
    public Employee? Current { get; private set; }
    public bool IsLoggedIn => Current != null;
    public bool IsManager => string.Equals(Current?.Role, "Manager", StringComparison.OrdinalIgnoreCase);
    public bool IsEmployee => IsLoggedIn;

    public void Set(Employee? employee) => Current = employee;
    public void Clear() => Current = null;
}
