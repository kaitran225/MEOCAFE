using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace POS.Avalonia.ViewModels;

public partial class ShellViewModel : ViewModelBase
{
    private readonly CurrentUserService _currentUser;
    private readonly Services.IViewModelResolver _resolver;
    private readonly Services.IAppNavigator _navigator;
    private readonly Services.PendingTableService _pendingTable;

    [ObservableProperty] private ViewModelBase? _currentContent;
    [ObservableProperty] private string _title = "MEOCAFE POS";
    [ObservableProperty] private NavItem? _selectedNavItem;

    public string CurrentUserName => _currentUser.Current?.Fullname ?? "";

    public ObservableCollection<NavItem> MenuItems { get; } = new();

    public ShellViewModel(CurrentUserService currentUser, Services.IViewModelResolver resolver, Services.IAppNavigator navigator, Services.PendingTableService pendingTable)
    {
        _currentUser = currentUser;
        _resolver = resolver;
        _navigator = navigator;
        _pendingTable = pendingTable;
        BuildMenu();
        _pendingTable.NavigateToOrderRequested += OnNavigateToOrderRequested;
        SelectDefault();
    }

    private void OnNavigateToOrderRequested()
    {
        var orderItem = MenuItems.FirstOrDefault(x => x.ViewModelType == typeof(OrderSelectionViewModel));
        if (orderItem != null) SelectedNavItem = orderItem;
    }

    private void BuildMenu()
    {
        MenuItems.Clear();
        if (_currentUser.IsManager)
        {
            MenuItems.Add(new NavItem("Dashboard", typeof(DashboardViewModel)));
            MenuItems.Add(new NavItem("Menu management", typeof(MenuManagementViewModel)));
            MenuItems.Add(new NavItem("Inventory", typeof(InventoryViewModel)));
            MenuItems.Add(new NavItem("Customers", typeof(CustomerManagementViewModel)));
            MenuItems.Add(new NavItem("Employee management", typeof(EmployeeManagementViewModel)));
            MenuItems.Add(new NavItem("Discount management", typeof(DiscountManagementViewModel)));
            MenuItems.Add(new NavItem("Shift management", typeof(ShiftManagementViewModel)));
            MenuItems.Add(new NavItem("Audit log", typeof(AuditLogViewModel)));
            MenuItems.Add(new NavItem("Reports", typeof(ReportsViewModel)));
        }
        MenuItems.Add(new NavItem("Table map", typeof(TableMapViewModel)));
        MenuItems.Add(new NavItem("Order", typeof(OrderSelectionViewModel)));
        MenuItems.Add(new NavItem("Kitchen display", typeof(KitchenDisplayViewModel)));
        MenuItems.Add(new NavItem("Order history", typeof(OrderHistoryViewModel)));
        MenuItems.Add(new NavItem("Shift register", typeof(ShiftRegisterViewModel)));
        MenuItems.Add(new NavItem("Settings", typeof(SettingsViewModel)));
    }

    private void SelectDefault()
    {
        if (MenuItems.Count > 0)
        {
            SelectedNavItem = MenuItems[0];
            Select(MenuItems[0]);
        }
    }

    partial void OnSelectedNavItemChanged(NavItem? value)
    {
        if (value != null) Select(value);
    }

    private void Select(NavItem item)
    {
        var vm = _resolver.Resolve(item.ViewModelType);
        if (vm != null)
        {
            CurrentContent = vm;
            Title = item.Label + " - MEOCAFE POS";
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _currentUser.Clear();
        _navigator.GoToLogin();
    }
}

public class NavItem
{
    public string Label { get; }
    public Type ViewModelType { get; }
    public NavItem(string label, Type viewModelType) { Label = label; ViewModelType = viewModelType; }
}
