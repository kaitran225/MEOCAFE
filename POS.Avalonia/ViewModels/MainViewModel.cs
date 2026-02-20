using CommunityToolkit.Mvvm.ComponentModel;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;
using POS.Avalonia;

namespace POS.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IEmployeeRepository _employeeRepo;
    private readonly CurrentUserService _currentUser;
    private readonly IViewModelResolver _resolver;

    [ObservableProperty] private ViewModelBase? _currentContent;

    public MainViewModel(IEmployeeRepository employeeRepo, CurrentUserService currentUser, IViewModelResolver resolver, IAppNavigator navigator)
    {
        _employeeRepo = employeeRepo;
        _currentUser = currentUser;
        _resolver = resolver;
        navigator.Register(this);
        // Bypass login: set default dev user and show shell
        _currentUser.Set(new Employee { Id = 0, Fullname = "Dev User", Username = "dev", Role = "Manager" });
        ShowShell();
    }

    internal static IAppNavigator GetNavigator() => Program.GetService<IAppNavigator>();

    public void ShowShell()
    {
        var shell = _resolver.Resolve(typeof(ShellViewModel));
        if (shell != null)
            CurrentContent = shell;
    }

    public void ShowLogin()
    {
        CurrentContent = new LoginViewModel(_employeeRepo, _currentUser, GetNavigator());
    }
}
