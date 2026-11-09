using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Services;
using System;

namespace POS.Avalonia.ViewModels;

public partial class ModuleHostViewModel : ViewModelBase
{
    private readonly IAppNavigator _navigator;
    private readonly IViewModelResolver _resolver;

    [ObservableProperty] private string _title = "";
    [ObservableProperty] private ViewModelBase? _moduleContent;

    public ModuleHostViewModel(IAppNavigator navigator, IViewModelResolver resolver)
    {
        _navigator = navigator;
        _resolver = resolver;
    }

    public void Open(Type viewModelType, string title)
    {
        Title = title;
        ModuleContent = _resolver.Resolve(viewModelType);
        if (ModuleContent is OrderSelectionViewModel order)
            order.ApplyPendingTableIfAny();
    }

    [RelayCommand]
  private void GoBack() => _navigator.GoHome();
}
