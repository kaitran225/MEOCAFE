using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using POS.Contracts.Services;
using POS.Models;
using POS.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Net.Mime.MediaTypeNames;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace POS.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page
{
    public RoleViewModel _roleViewModel;
    public ShellViewModel _shellViewModel { get; set; }

    public LoginPage()
    {
        InitializeComponent();
        _roleViewModel = App.GetService<RoleViewModel>();
        DataContext = _roleViewModel;
    }

    private void Manager_Click(object sender, RoutedEventArgs e)
    {
        _roleViewModel.UpdateRole("Manager");
    }

    private void Cashier_Click(object sender, RoutedEventArgs e)
    {
        _roleViewModel.UpdateRole("Cashier");
        CurrentUser.setCurrent(UsernameTextBox.Text);
    }
}
