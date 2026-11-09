using CommunityToolkit.Mvvm.ComponentModel;
using POS.Avalonia.Services;

namespace POS.Avalonia.ViewModels;

public partial class DebugViewModel : ViewModelBase
{
    private readonly AppWindowDebugInfo _debugInfo;

    public DebugViewModel(AppWindowDebugInfo debugInfo)
    {
        _debugInfo = debugInfo;
    }

    public string Title => "Debug";
    public AppWindowDebugInfo DebugInfo => _debugInfo;
}
