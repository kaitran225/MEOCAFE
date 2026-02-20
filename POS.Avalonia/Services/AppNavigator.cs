namespace POS.Avalonia.Services;

public sealed class AppNavigator : IAppNavigator
{
    private ViewModels.MainViewModel? _main;

    public void Register(ViewModels.MainViewModel mainViewModel) => _main = mainViewModel;
    public void GoToShell() => _main?.ShowShell();
    public void GoToLogin() => _main?.ShowLogin();
}
