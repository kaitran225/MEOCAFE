using System;
using CommunityToolkit.Mvvm.ComponentModel;
using POS.Avalonia.Services;

namespace POS.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IViewModelResolver _resolver;

    [ObservableProperty] private ViewModelBase? _currentContent;

    public BentoHomeViewModel Home { get; }
    public ModuleHostViewModel ModuleHost { get; }

    public MainViewModel(
        IViewModelResolver resolver,
        IAppNavigator navigator,
        BentoHomeViewModel home,
        ModuleHostViewModel moduleHost)
    {
        _resolver = resolver;
        Home = home;
        ModuleHost = moduleHost;
        navigator.Register(this);
        ShowHome();
    }

    public void ShowHome() => CurrentContent = Home;

    public void ShowModule(Type viewModelType, string title)
    {
        ModuleHost.Open(viewModelType, title);
        CurrentContent = ModuleHost;
    }
}
