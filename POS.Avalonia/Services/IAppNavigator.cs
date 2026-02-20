namespace POS.Avalonia.Services;

public interface IAppNavigator
{
    void Register(ViewModels.MainViewModel mainViewModel);
    void GoToShell();
    void GoToLogin();
}
