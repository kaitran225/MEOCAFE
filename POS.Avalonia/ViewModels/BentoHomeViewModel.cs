using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Localization;
using POS.Avalonia.Localization;
using POS.Avalonia.Services;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace POS.Avalonia.ViewModels;

/// <summary>
/// Home / module wall. One square tile per module, ordered by how often it is used —
/// no group headers, no dividers; the order and the spacing carry the grouping.
/// </summary>
public partial class BentoHomeViewModel : ViewModelBase
{
    private readonly IAppNavigator _navigator;
    private readonly IStringLocalizer<StringsResource> _loc;

    public ObservableCollection<BentoTile> Tiles { get; } = new();

    public string TodayCaption => "Hôm nay, " + DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    public BentoHomeViewModel(IAppNavigator navigator, IStringLocalizer<StringsResource> loc)
    {
        _navigator = navigator;
        _loc = loc;
        BuildTiles();
    }

    [RelayCommand]
    private void OpenTile(BentoTile? tile)
    {
        if (tile is null) return;
        _navigator.OpenModule(tile.ViewModelType, tile.Title);
    }

    private string Nav(string key, string fallback) => _loc[key].Value ?? fallback;

    private void BuildTiles()
    {
        Tiles.Clear();

        // Ordered by frequency of use: selling first, then the shop's daily operations,
        // then management, then reporting and system screens.
        Tiles.Add(Tile("POS Bán hàng", "Gọi món & thanh toán", typeof(OrderSelectionViewModel), "IconPointOfSale"));
        Tiles.Add(Tile(Nav("Dashboard", "Tổng quan"), "Doanh thu hôm nay", typeof(DashboardViewModel), "IconDashboard"));
        Tiles.Add(Tile(Nav("TableMap", "Sơ đồ bàn"), "Bàn trống & đang phục vụ", typeof(TableMapViewModel), "IconTableRestaurant"));
        Tiles.Add(Tile(Nav("MenuManagement", "Quản lý thực đơn"), "Món & danh mục", typeof(MenuManagementViewModel), "IconRecipe"));
        Tiles.Add(Tile(Nav("Inventory", "Kho hàng"), "Tồn kho nguyên liệu", typeof(InventoryViewModel), "IconInventory"));
        Tiles.Add(Tile(Nav("RecipeBook", "Công thức"), "Công thức pha chế", typeof(RecipeBookViewModel), "IconKitchen"));
        Tiles.Add(Tile(Nav("MusicPlayer", "Âm nhạc"), "Nhạc nền cửa hàng", typeof(MusicPlayerViewModel), "IconMusic"));
        Tiles.Add(Tile(Nav("DiscountManagement", "Khuyến mãi"), "Giảm giá & combo", typeof(DiscountManagementViewModel), "IconDiscount"));
        Tiles.Add(Tile(Nav("ShiftManagement", "Ca làm việc"), "Phân ca & lịch làm", typeof(ShiftManagementViewModel), "IconShift"));
        Tiles.Add(Tile(Nav("ShiftRegister", "Chấm công"), "Vào ca & kết ca", typeof(ShiftRegisterViewModel), "IconEmployees"));
        Tiles.Add(Tile(Nav("BankTransaction", "Thu chi"), "Giao dịch ngân hàng", typeof(BankTransactionViewModel), "IconCash"));
        Tiles.Add(Tile(Nav("CostManagement", "Chi phí"), "Giá vốn & chi phí", typeof(CostManagementViewModel), "IconCost"));
        Tiles.Add(Tile(Nav("WasteLog", "Hao hụt"), "Nhật ký hao hụt", typeof(WasteLogViewModel), "IconWaste"));
        Tiles.Add(Tile(Nav("DailySales", "Báo cáo ngày"), "Chốt ca & doanh thu ngày", typeof(DailySalesViewModel), "IconReport"));
        Tiles.Add(Tile(Nav("Reports", "Báo cáo"), "Phân tích bán hàng", typeof(ReportsViewModel), "IconBarChart"));
        Tiles.Add(Tile(Nav("OrderHistory", "Lịch sử đơn"), "Tra cứu & in lại", typeof(OrderHistoryViewModel), "IconHistory"));
        Tiles.Add(Tile(Nav("ProblemReports", "Sự cố"), "Phản ánh & xử lý", typeof(ProblemReportsViewModel), "IconWarning"));
        Tiles.Add(Tile(Nav("AuditLog", "Nhật ký"), "Hoạt động hệ thống", typeof(AuditLogViewModel), "IconAuditLog"));
        Tiles.Add(Tile(Nav("Settings", "Cài đặt"), "Cấu hình cửa hàng", typeof(SettingsViewModel), "IconSettings"));
        Tiles.Add(Tile(Nav("AiChat", "Trợ lý AI"), "Hỏi đáp & gợi ý", typeof(AiChatViewModel), "IconChat"));
        Tiles.Add(Tile(Nav("StyleGuide", "Giao diện"), "Bộ thành phần chuẩn", typeof(StyleGuideViewModel), "IconStar"));
        Tiles.Add(Tile(Nav("Debug", "Gỡ lỗi"), "Chẩn đoán hệ thống", typeof(DebugViewModel), "IconInfo"));
    }

    private static BentoTile Tile(string title, string? subtitle, Type viewModelType, string iconKey)
        => new()
        {
            Title = title,
            Subtitle = subtitle,
            ViewModelType = viewModelType,
            IconKey = iconKey
        };
}
