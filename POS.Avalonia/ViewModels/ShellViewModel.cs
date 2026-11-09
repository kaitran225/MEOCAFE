using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Localization;
using POS.Avalonia.Localization;
using POS.Avalonia.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace POS.Avalonia.ViewModels;

public partial class ShellViewModel : ViewModelBase
{
    private readonly CurrentUserService _currentUser;
    private readonly IViewModelResolver _resolver;
    private readonly PendingTableService _pendingTable;
    private readonly IStringLocalizer<StringsResource> _loc;

    [ObservableProperty] private ViewModelBase? _currentContent;
    [ObservableProperty] private string _title = "HienXanh POS";
    [ObservableProperty] private string _pageTitle = "";
    [ObservableProperty] private string _todayDate = "";
    [ObservableProperty] private NavItem? _selectedNavItem;
    [ObservableProperty] private bool _hasMenuItems = false;

    public string AppTitle => _loc["App_Title"].Value ?? "HienXanh";
    public string AppSubtitle => _loc["App_Subtitle"].Value ?? "";
    public string StatusOnline => _loc["Status_Online"].Value ?? "";
    public string CurrentUserName => _currentUser.Current?.Fullname ?? "HienXanh";

    public ObservableCollection<NavItem> MenuItems { get; } = new();

    public ShellViewModel(
        CurrentUserService currentUser,
        IViewModelResolver resolver,
        PendingTableService pendingTable,
        IStringLocalizer<StringsResource> loc)
    {
        _currentUser = currentUser;
        _resolver = resolver;
        _pendingTable = pendingTable;
        _loc = loc;
        _todayDate = DateTime.Now.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CurrentUICulture);
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

        // Dashboard
        MenuItems.Add(new NavItem(_loc["Dashboard"], typeof(DashboardViewModel), "IconDashboard"));

        // ── VẬN HÀNH ──
        MenuItems.Add(new NavItem(_loc["MenuManagement"], typeof(MenuManagementViewModel), "IconMenu"));
        MenuItems.Add(new NavItem(_loc["OrderSelection"], typeof(OrderSelectionViewModel), "IconReceipt"));
        MenuItems.Add(new NavItem(_loc["TableMap"], typeof(TableMapViewModel), "IconMap"));
        MenuItems.Add(new NavItem(_loc["Inventory"], typeof(InventoryViewModel), "IconPackage"));
        MenuItems.Add(new NavItem(_loc["MusicPlayer"], typeof(MusicPlayerViewModel), "IconMusic"));

        // ── QUẢN LÝ ──
        MenuItems.Add(new NavItem(_loc["DiscountManagement"], typeof(DiscountManagementViewModel), "IconTag"));
        MenuItems.Add(new NavItem(_loc["RecipeBook"], typeof(RecipeBookViewModel), "IconBook"));
        MenuItems.Add(new NavItem(_loc["ShiftManagement"], typeof(ShiftManagementViewModel), "IconShift"));
        MenuItems.Add(new NavItem(_loc["ShiftRegister"], typeof(ShiftRegisterViewModel), "IconClipboard"));
        MenuItems.Add(new NavItem(_loc["BankTransaction"], typeof(BankTransactionViewModel), "IconBank"));
        MenuItems.Add(new NavItem(_loc["CostManagement"], typeof(CostManagementViewModel), "IconChartBar"));
        MenuItems.Add(new NavItem(_loc["WasteLog"], typeof(WasteLogViewModel), "IconTrash"));

        // ── HỆ THỐNG ──
        MenuItems.Add(new NavItem(_loc["Settings"], typeof(SettingsViewModel), "IconSettings"));
        MenuItems.Add(new NavItem(_loc["Reports"], typeof(ReportsViewModel), "IconReport"));
        MenuItems.Add(new NavItem(_loc["DailySales"], typeof(DailySalesViewModel), "IconChartLine"));
        MenuItems.Add(new NavItem(_loc["OrderHistory"], typeof(OrderHistoryViewModel), "IconHistory"));
        MenuItems.Add(new NavItem(_loc["ProblemReports"], typeof(ProblemReportsViewModel), "IconAlert"));
        MenuItems.Add(new NavItem(_loc["AuditLog"], typeof(AuditLogViewModel), "IconClipboardList"));
        MenuItems.Add(new NavItem(_loc["StyleGuide"], typeof(StyleGuideViewModel), "IconPalette"));
        MenuItems.Add(new NavItem(_loc["AiChat"], typeof(AiChatViewModel), "IconSparkles"));
        MenuItems.Add(new NavItem(_loc["Debug"], typeof(DebugViewModel), "IconBug"));
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
            PageTitle = item.Label;
            Title = item.Label + " - HienXanh";
        }
    }
}

public class NavItem
{
    public string Label { get; }
    public Type ViewModelType { get; }
    public string IconKey { get; }

    public NavItem(LocalizedString label, Type viewModelType, string iconKey = "IconDashboard")
    {
        Label = label.Value ?? viewModelType.Name;
        ViewModelType = viewModelType;
        IconKey = iconKey;
    }
}
