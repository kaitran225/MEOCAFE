using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Localization;
using POS.Avalonia.Localization;
using POS.Avalonia.Services;
using System;
using System.Collections.ObjectModel;

namespace POS.Avalonia.ViewModels;

public partial class BentoHomeViewModel : ViewModelBase
{
    private readonly IAppNavigator _navigator;
    private readonly IStringLocalizer<StringsResource> _loc;

    public ObservableCollection<BentoTile> OperationTiles { get; } = new();
    public ObservableCollection<BentoTile> ManagementTiles { get; } = new();
    public ObservableCollection<BentoTile> SystemTiles { get; } = new();

    public BentoHomeViewModel(IAppNavigator navigator, IStringLocalizer<StringsResource> loc)
    {
        _navigator = navigator;
        _loc = loc;
        BuildTiles();
    }

    private void BuildTiles()
    {
        OperationTiles.Clear();
        ManagementTiles.Clear();
        SystemTiles.Clear();

        OperationTiles.Add(new BentoTile
        {
            Title = "Đặt bàn",
            Subtitle = "Quản lý bàn ăn",
            IconKey = "IconTableRestaurant",
            ViewModelType = typeof(DashboardViewModel),
            IsPrimary = true,
            GridColumnSpan = 2,
            GridRowSpan = 1
        });
        OperationTiles.Add(new BentoTile
        {
            Title = "POS Bán hàng",
            Subtitle = "Thanh toán nhanh",
            IconKey = "IconPointOfSale",
            ViewModelType = typeof(DashboardViewModel),
            IsPrimary = false
        });
        OperationTiles.Add(new BentoTile
        {
            Title = "Món ăn",
            Subtitle = "Menu & công thức",
            IconKey = "IconRecipe",
            ViewModelType = typeof(DashboardViewModel)
        });
        OperationTiles.Add(new BentoTile
        {
            Title = "Kho hàng",
            Subtitle = "Tồn kho nguyên liệu",
            IconKey = "IconInventory",
            ViewModelType = typeof(DashboardViewModel)
        });

        ManagementTiles.Add(new BentoTile
        {
            Title = "Báo cáo doanh thu",
            Subtitle = "Thống kê bán hàng",
            IconKey = "IconBarChart",
            ViewModelType = typeof(DashboardViewModel)
        });
        ManagementTiles.Add(new BentoTile
        {
            Title = "Nhân viên",
            Subtitle = "Ca làm & lương",
            IconKey = "IconEmployees",
            ViewModelType = typeof(DashboardViewModel)
        });
        ManagementTiles.Add(new BentoTile
        {
            Title = "Khách hàng",
            Subtitle = "Thành viên & điểm",
            IconKey = "IconCustomers",
            ViewModelType = typeof(DashboardViewModel)
        });
        ManagementTiles.Add(new BentoTile
        {
            Title = "Khuyến mãi",
            Subtitle = "Giảm giá & combo",
            IconKey = "IconDiscount",
            ViewModelType = typeof(DashboardViewModel)
        });

        SystemTiles.Add(new BentoTile
        {
            Title = "Cài đặt",
            Subtitle = "Cấu hình hệ thống",
            IconKey = "IconSettings",
            ViewModelType = typeof(DashboardViewModel)
        });
        SystemTiles.Add(new BentoTile
        {
            Title = "Nhật ký",
            Subtitle = "Hoạt động hệ thống",
            IconKey = "IconAuditLog",
            ViewModelType = typeof(DashboardViewModel)
        });
        SystemTiles.Add(new BentoTile
        {
            Title = "Sao lưu",
            Subtitle = "Backup & restore",
            IconKey = "IconSave",
            ViewModelType = typeof(DashboardViewModel)
        });
        SystemTiles.Add(new BentoTile
        {
            Title = "Thông báo",
            Subtitle = "Cảnh báo hệ thống",
            IconKey = "IconNotification",
            ViewModelType = typeof(DashboardViewModel)
        });
    }
}
