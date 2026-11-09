using System;

namespace POS.Avalonia.ViewModels;

public sealed class BentoTile
{
    public string Title { get; init; } = "";
    public string? Subtitle { get; init; }
    public string IconKey { get; init; } = "IconDashboard";
    public Type ViewModelType { get; init; } = typeof(DashboardViewModel);
    public bool IsPrimary { get; init; }
    public int GridColumnSpan { get; init; } = 1;
    public int GridRowSpan { get; init; } = 1;
}
