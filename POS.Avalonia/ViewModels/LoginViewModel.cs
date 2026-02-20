using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IEmployeeRepository _employeeRepo;
    private readonly CurrentUserService _currentUser;
    private readonly IAppNavigator _navigator;

    [ObservableProperty] private string _username = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NotBusy))]
    private bool _isBusy;
    public bool NotBusy => !IsBusy;

    public LoginViewModel(IEmployeeRepository employeeRepo, CurrentUserService currentUser, IAppNavigator navigator)
    {
        _employeeRepo = employeeRepo;
        _currentUser = currentUser;
        _navigator = navigator;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = "";
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter username and password.";
            return;
        }
        IsBusy = true;
        try
        {
            var employee = await _employeeRepo.GetByUsernamePasswordAsync(Username.Trim(), Password, default).ConfigureAwait(true);
            if (employee != null)
            {
                _currentUser.Set(employee);
                _navigator.GoToShell();
            }
            else
                ErrorMessage = "Invalid username or password.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
