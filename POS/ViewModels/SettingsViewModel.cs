using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace POS.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private bool isLoggedOut = true;
    private int _id;
    private string _fullname;
    private string _username;
    private string _role;

    public bool IsLoggedOut
    {
        get => isLoggedOut;
        set
        {
            SetProperty(ref isLoggedOut, value);
            OnPropertyChanged(nameof(IsLoggedOut));
        }
    }

    public Visibility Logout => !IsLoggedOut ? Visibility.Visible : Visibility.Collapsed;
    public Visibility Login => IsLoggedOut ? Visibility.Visible : Visibility.Collapsed;

    public int Id
    {
        get => _id;
        set
        {
            SetProperty(ref _id, value);
            OnPropertyChanged(nameof(Id));
        }
    }

    public string Fullname
    {
        get => _fullname;
        set
        {
            SetProperty(ref _fullname, value);
            OnPropertyChanged(nameof(Fullname));
        }
    }

    public string Username
    {
        get => _username;
        set
        {
            SetProperty(ref _username, value);
            OnPropertyChanged(nameof(Username));
        }
    }

    public string Role
    {
        get => _role;
        set
        {
            SetProperty(ref _role, value);
            OnPropertyChanged(nameof(Role));
        }
    }
}
