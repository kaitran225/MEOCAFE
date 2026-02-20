using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;

namespace POS.ViewModels;

public partial class RoleViewModel : ObservableRecipient
{
    private string _currentRole = "Login";
    public string CurrentRole
    {
        get => _currentRole;
        set
        {
            SetProperty(ref _currentRole, value);
            OnPropertyChanged(nameof(ManagerVisibility));
            OnPropertyChanged(nameof(CashierVisibility));
        }
    }

    public Visibility LoginVisibility => CurrentRole == "Login" ? Visibility.Visible : Visibility.Collapsed;
    public Visibility ManagerVisibility => CurrentRole == "Manager" ? Visibility.Visible : Visibility.Collapsed;
    public Visibility CashierVisibility => CurrentRole == "Cashier" ? Visibility.Visible : Visibility.Collapsed;

    public void UpdateRole(string role)
    {
        CurrentRole = role;
    }
}