using System;
using POS.Core.Models;

namespace POS.Avalonia.Services;

/// <summary>When navigating from Table map to Order with "New order at table", this holds the table until Order consumes it.</summary>
public sealed class PendingTableService
{
    private Table? _pending;
    public event Action? NavigateToOrderRequested;

    public void Set(Table? table) => _pending = table;
    public Table? GetAndClear() { var t = _pending; _pending = null; return t; }
    public void SetAndRequestNavigateToOrder(Table table) { _pending = table; NavigateToOrderRequested?.Invoke(); }
}
