using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class OrderSelectionViewModel : ViewModelBase
{
    private readonly ICategoryRepository _categoryRepo;
    private readonly IMenuItemRepository _menuItemRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IOrderDetailRepository _detailRepo;
    private readonly IOrderDetailOptionRepository _optionRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly ITableRepository _tableRepo;
    private readonly IReceiptService _receiptService;
    private readonly IPrintService _printService;
    private readonly Services.IAuditLogger _auditLogger;
    private readonly Services.PendingTableService _pendingTable;
    private readonly decimal _taxRate;
    private readonly decimal _loyaltyEarnPerAmount;
    private readonly int _loyaltyEarnPoints;
    private readonly decimal _loyaltyValuePerPoint;

    [ObservableProperty] private ObservableCollection<Category> _categories = new();
    [ObservableProperty] private ObservableCollection<MenuItem> _filteredMenuItems = new();
    [ObservableProperty] private Category? _selectedCategory;
    [ObservableProperty] private MenuItem? _selectedMenuItem;
    [ObservableProperty] private ObservableCollection<BillViewModel> _bills = new();
    [ObservableProperty] private int _currentBillIndex;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _searchTerm = "";
    [ObservableProperty] private BillLineViewModel? _customizationLine;
    [ObservableProperty] private bool _isCustomizationOpen;
    [ObservableProperty] private bool _isPaymentOpen;
    [ObservableProperty] private decimal _paymentAmount;
    [ObservableProperty] private bool _isVoidLineDialogOpen;
    [ObservableProperty] private BillLineViewModel? _voidLineTarget;
    [ObservableProperty] private string _voidReason = "";
    [ObservableProperty] private bool _isVoidBillDialogOpen;
    [ObservableProperty] private string _voidBillReason = "";
    [ObservableProperty] private ObservableCollection<BillViewModel> _heldBills = new();
    [ObservableProperty] private bool _isRecallOpen;
    [ObservableProperty] private Customer? _selectedCustomer;
    [ObservableProperty] private bool _isCustomerSearchOpen;
    [ObservableProperty] private string _customerSearchTerm = "";
    [ObservableProperty] private ObservableCollection<Customer> _customerSearchResults = new();
    [ObservableProperty] private bool _isNewCustomerOpen;
    [ObservableProperty] private string _newCustomerName = "";
    [ObservableProperty] private string _newCustomerPhone = "";
    [ObservableProperty] private string _newCustomerEmail = "";
    [ObservableProperty] private string _newCustomerAddress = "";
    [ObservableProperty] private bool _isRedeemPointsOpen;
    [ObservableProperty] private string _pointsToRedeemText = "0";
    [ObservableProperty] private ObservableCollection<Table> _tables = new();
    [ObservableProperty] private string _deliveryAddress = "";
    [ObservableProperty] private string _deliveryFeeText = "0";

    public string[] IceLevels { get; } = { "None", "Less", "Normal", "More" };
    public string[] ServiceTypes { get; } = { "Dine-in", "Takeaway", "Delivery" };
    public string[] SugarLevels { get; } = { "None", "Less", "Normal", "More" };
    public string[] SizeOptions { get; } = { "S", "M", "L" };

    public BillViewModel? CurrentBill => CurrentBillIndex >= 0 && CurrentBillIndex < Bills.Count ? Bills[CurrentBillIndex] : null;
    public string CustomerDisplayText => SelectedCustomer == null ? "" : $"{SelectedCustomer.Name} ({SelectedCustomer.Point} pts)";
    public bool HasCustomer => SelectedCustomer != null;

    public OrderSelectionViewModel(
        ICategoryRepository categoryRepo,
        IMenuItemRepository menuItemRepo,
        IOrderRepository orderRepo,
        IOrderDetailRepository detailRepo,
        IOrderDetailOptionRepository optionRepo,
        ICustomerRepository customerRepo,
        ITableRepository tableRepo,
        IReceiptService receiptService,
        IPrintService printService,
        Services.IAuditLogger auditLogger,
        Services.PendingTableService pendingTable,
        IConfiguration configuration)
    {
        _categoryRepo = categoryRepo;
        _menuItemRepo = menuItemRepo;
        _orderRepo = orderRepo;
        _detailRepo = detailRepo;
        _optionRepo = optionRepo;
        _customerRepo = customerRepo;
        _tableRepo = tableRepo;
        _receiptService = receiptService;
        _printService = printService;
        _auditLogger = auditLogger;
        _pendingTable = pendingTable;
        _tableRepo = tableRepo;
        _taxRate = decimal.TryParse(configuration["Tax:Rate"], out var tr) ? tr : 0.1m;
        _loyaltyEarnPerAmount = decimal.TryParse(configuration["Loyalty:EarnPerAmount"], out var ea) ? ea : 10000m;
        _loyaltyEarnPoints = int.TryParse(configuration["Loyalty:EarnPoints"], out var ep) ? ep : 1;
        _loyaltyValuePerPoint = decimal.TryParse(configuration["Loyalty:ValuePerPoint"], out var vp) ? vp : 100m;
        _ = LoadAsync();
        var pending = _pendingTable.GetAndClear();
        if (pending != null) NewBillForTable(pending);
        else NewBill();
    }

    partial void OnSelectedCategoryChanged(Category? value) => FilterMenuItems();
    partial void OnSearchTermChanged(string value) => FilterMenuItems();
    partial void OnCurrentBillIndexChanged(int value) => OnPropertyChanged(nameof(CurrentBill));
    partial void OnSelectedCustomerChanged(Customer? value) { OnPropertyChanged(nameof(CustomerDisplayText)); OnPropertyChanged(nameof(HasCustomer)); }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var cats = await _categoryRepo.GetAllAsync(default).ConfigureAwait(true);
            var items = await _menuItemRepo.GetAllAsync(default).ConfigureAwait(true);
            var tbls = await _tableRepo.GetAllAsync(default).ConfigureAwait(true);
            Categories = new ObservableCollection<Category>(cats);
            foreach (var c in Categories)
                c.MenuItems = items.Where(m => m.CategoryId == c.Id).ToList();
            _allMenuItems = new ObservableCollection<MenuItem>(items);
            Tables = new ObservableCollection<Table>(tbls);
            FilterMenuItems();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private ObservableCollection<MenuItem> _allMenuItems = new();

    private void FilterMenuItems()
    {
        var list = _allMenuItems.AsEnumerable();
        if (SelectedCategory != null)
            list = list.Where(m => m.CategoryId == SelectedCategory.Id);
        if (!string.IsNullOrWhiteSpace(SearchTerm))
            list = list.Where(m => (m.Name ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        FilteredMenuItems = new ObservableCollection<MenuItem>(list.ToList());
    }

    [RelayCommand]
    private void NewBill()
    {
        NewBillForTable(null);
    }

    [RelayCommand]
    private void NewBillForTable(Table? table)
    {
        var label = table != null ? table.Name : ("Bill " + (Bills.Count + 1));
        var bill = new BillViewModel
        {
            Label = label,
            TaxRate = _taxRate,
            ServiceType = table != null ? "Dine-in" : "Dine-in",
            TableId = table?.Id,
            TableName = table?.Name ?? "",
            DeliveryFee = 0
        };
        Bills.Add(bill);
        CurrentBillIndex = Bills.Count - 1;
    }

    [RelayCommand]
    private void SetServiceType(string? type)
    {
        if (CurrentBill == null || string.IsNullOrEmpty(type)) return;
        CurrentBill.ServiceType = type;
        CurrentBill.TableId = null;
        CurrentBill.TableName = "";
        CurrentBill.DeliveryAddress = null;
        CurrentBill.DeliveryFee = 0;
        if (type == "Delivery") { CurrentBill.DeliveryAddress = DeliveryAddress; CurrentBill.DeliveryFee = decimal.TryParse(DeliveryFeeText, out var f) ? f : 0; }
        CurrentBill.RecalcTotals();
    }

    [RelayCommand]
    private void SetTable(Table? table)
    {
        if (CurrentBill == null) return;
        CurrentBill.ServiceType = "Dine-in";
        CurrentBill.TableId = table?.Id;
        CurrentBill.TableName = table?.Name ?? "";
        CurrentBill.Label = string.IsNullOrEmpty(CurrentBill.TableName) ? "Bill " + (Bills.Count) : CurrentBill.TableName;
        CurrentBill.DeliveryAddress = null;
        CurrentBill.DeliveryFee = 0;
        CurrentBill.RecalcTotals();
    }

    [RelayCommand]
    private void ApplyDeliveryToCurrentBill()
    {
        if (CurrentBill == null) return;
        CurrentBill.ServiceType = "Delivery";
        CurrentBill.DeliveryAddress = DeliveryAddress;
        CurrentBill.DeliveryFee = decimal.TryParse(DeliveryFeeText, out var f) ? f : 0;
        CurrentBill.TableId = null;
        CurrentBill.TableName = "";
        CurrentBill.RecalcTotals();
    }

    [RelayCommand(CanExecute = nameof(CanAddProduct))]
    private void AddProduct(MenuItem? item)
    {
        if (item == null || CurrentBill == null) return;
        var existing = CurrentBill.Lines.FirstOrDefault(l => l.MenuItemId == item.Id && !l.IsCombo);
        if (existing != null)
        {
            existing.Quantity++;
            existing.RecalcTotal();
        }
        else
        {
            var line = new BillLineViewModel
            {
                MenuItemId = item.Id,
                ProductName = item.Name ?? "",
                Quantity = 1,
                UnitPrice = item.SellPrice,
                LineTotal = item.SellPrice,
                ParentBill = CurrentBill
            };
            CurrentBill.Lines.Add(line);
        }
        CurrentBill.RecalcTotals();
        AddProductCommand.NotifyCanExecuteChanged();
    }

    private bool CanAddProduct(MenuItem? item) => item != null && CurrentBill != null;

    [RelayCommand]
    private void RemoveLine(BillLineViewModel? line)
    {
        if (line == null || CurrentBill == null) return;
        CurrentBill.Lines.Remove(line);
        CurrentBill.RecalcTotals();
    }

    [RelayCommand]
    private void CustomizeLine(BillLineViewModel? line)
    {
        if (line == null) return;
        CustomizationLine = line;
        IsCustomizationOpen = true;
    }

    [RelayCommand]
    private void CloseCustomization()
    {
        IsCustomizationOpen = false;
        CustomizationLine = null;
    }

    [RelayCommand]
    private void SetCustomizationIce(string? level)
    {
        if (CustomizationLine != null && level != null) CustomizationLine.IceLevel = level;
    }

    [RelayCommand]
    private void SetCustomizationSugar(string? level)
    {
        if (CustomizationLine != null && level != null) CustomizationLine.SugarLevel = level;
    }

    [RelayCommand]
    private void SetCustomizationSize(string? size)
    {
        if (CustomizationLine != null && size != null) CustomizationLine.Size = size;
    }

    [RelayCommand(CanExecute = nameof(CanPay))]
    private void Pay()
    {
        if (CurrentBill == null) return;
        PaymentAmount = CurrentBill.GrandTotal;
        IsPaymentOpen = true;
    }

    [RelayCommand]
    private async Task CompletePaymentAsync()
    {
        if (CurrentBill == null) return;
        try
        {
            var order = new Order
            {
                TotalPrice = CurrentBill.GrandTotal,
                Note = null,
                PhoneNumber = SelectedCustomer?.PhoneNumber,
                CustomerId = SelectedCustomer?.Id,
                TableId = CurrentBill.TableId,
                ServiceType = CurrentBill.ServiceType ?? "Dine-in",
                DeliveryAddress = CurrentBill.DeliveryAddress,
                DeliveryFee = CurrentBill.DeliveryFee
            };
            var orderId = await _orderRepo.AddAsync(order, default).ConfigureAwait(true);
            foreach (var line in CurrentBill.Lines)
            {
                var detail = new OrderDetail
                {
                    OrderId = orderId,
                    MenuItemId = line.MenuItemId,
                    Quantity = line.Quantity,
                    Price = line.UnitPrice,
                    IsCombo = line.IsCombo
                };
                await _detailRepo.AddAsync(detail, default).ConfigureAwait(false);
                await AddLineOptionsAsync(detail.Id, line).ConfigureAwait(false);
                await _menuItemRepo.DecreaseQuantityAsync(line.MenuItemId, line.Quantity, default).ConfigureAwait(false);
            }
            await _orderRepo.UpdateKitchenStatusAsync(orderId, "sent", default).ConfigureAwait(false);
            if (SelectedCustomer != null)
            {
                var earned = (int)(CurrentBill.GrandTotal / _loyaltyEarnPerAmount) * _loyaltyEarnPoints;
                if (earned > 0)
                    await _customerRepo.AddPointsAsync(SelectedCustomer.PhoneNumber, earned, default).ConfigureAwait(false);
                if (CurrentBill.PointsRedeemed > 0)
                    await _customerRepo.AddPointsAsync(SelectedCustomer.PhoneNumber, -CurrentBill.PointsRedeemed, default).ConfigureAwait(false);
            }
            var receiptData = await _receiptService.BuildReceiptFromOrderAsync(orderId, "Cash", default).ConfigureAwait(true);
            var receiptText = _receiptService.FormatReceiptAsText(receiptData);
            _printService.PrintReceipt(receiptText);
        }
        finally
        {
            Bills.Remove(CurrentBill);
            if (CurrentBillIndex >= Bills.Count) CurrentBillIndex = Math.Max(0, Bills.Count - 1);
            if (Bills.Count == 0) NewBill();
            IsPaymentOpen = false;
            PaymentAmount = 0;
        }
    }

    private async Task AddLineOptionsAsync(int orderDetailId, BillLineViewModel line)
    {
        await _optionRepo.AddAsync(new OrderDetailOption { OrderDetailId = orderDetailId, OptionType = "ice", Note = line.IceLevel }, default).ConfigureAwait(false);
        await _optionRepo.AddAsync(new OrderDetailOption { OrderDetailId = orderDetailId, OptionType = "sugar", Note = line.SugarLevel }, default).ConfigureAwait(false);
        await _optionRepo.AddAsync(new OrderDetailOption { OrderDetailId = orderDetailId, OptionType = "size", Note = line.Size }, default).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(line.Note))
            await _optionRepo.AddAsync(new OrderDetailOption { OrderDetailId = orderDetailId, OptionType = "note", Note = line.Note }, default).ConfigureAwait(false);
    }

    [RelayCommand]
    private void CancelPayment()
    {
        IsPaymentOpen = false;
    }

    private bool CanPay() => CurrentBill != null && CurrentBill.Lines.Count > 0;

    [RelayCommand]
    private void RequestVoidLine(BillLineViewModel? line)
    {
        if (line == null || CurrentBill == null) return;
        VoidLineTarget = line;
        VoidReason = "";
        IsVoidLineDialogOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmVoidLineAsync()
    {
        if (VoidLineTarget == null || CurrentBill == null) return;
        _ = _auditLogger.LogAsync("VoidLine", "OrderDetail", VoidLineTarget.MenuItemId, VoidReason).ConfigureAwait(false);
        CurrentBill.Lines.Remove(VoidLineTarget);
        CurrentBill.RecalcTotals();
        IsVoidLineDialogOpen = false;
        VoidLineTarget = null;
        VoidReason = "";
        await Task.CompletedTask.ConfigureAwait(true);
    }

    [RelayCommand]
    private void CancelVoidLine()
    {
        IsVoidLineDialogOpen = false;
        VoidLineTarget = null;
    }

    [RelayCommand(CanExecute = nameof(CanVoidBill))]
    private void RequestVoidBill()
    {
        if (CurrentBill == null) return;
        VoidBillReason = "";
        IsVoidBillDialogOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmVoidBillAsync()
    {
        if (CurrentBill == null) return;
        _ = _auditLogger.LogAsync("VoidBill", null, null, VoidBillReason).ConfigureAwait(false);
        Bills.Remove(CurrentBill);
        if (CurrentBillIndex >= Bills.Count) CurrentBillIndex = Math.Max(0, Bills.Count - 1);
        if (Bills.Count == 0) NewBill();
        IsVoidBillDialogOpen = false;
        VoidBillReason = "";
        await Task.CompletedTask.ConfigureAwait(true);
    }

    [RelayCommand]
    private void CancelVoidBill()
    {
        IsVoidBillDialogOpen = false;
    }

    private bool CanVoidBill() => CurrentBill != null;

    [RelayCommand(CanExecute = nameof(CanHold))]
    private void HoldBill()
    {
        if (CurrentBill == null) return;
        HeldBills.Add(CurrentBill);
        Bills.Remove(CurrentBill);
        if (CurrentBillIndex >= Bills.Count) CurrentBillIndex = Math.Max(0, Bills.Count - 1);
        if (Bills.Count == 0) NewBill();
    }

    private bool CanHold() => CurrentBill != null && CurrentBill.Lines.Count > 0;

    [RelayCommand]
    private void OpenRecall()
    {
        IsRecallOpen = true;
    }

    [RelayCommand]
    private void CloseRecall()
    {
        IsRecallOpen = false;
    }

    [RelayCommand]
    private void RecallBill(BillViewModel? bill)
    {
        if (bill == null) return;
        HeldBills.Remove(bill);
        Bills.Add(bill);
        CurrentBillIndex = Bills.Count - 1;
        IsRecallOpen = false;
    }

    [RelayCommand]
    private void OpenCustomerSearch() => IsCustomerSearchOpen = true;

    [RelayCommand]
    private void CloseCustomerSearch()
    {
        IsCustomerSearchOpen = false;
        CustomerSearchResults = new ObservableCollection<Customer>();
    }

    [RelayCommand]
    private async Task SearchCustomersAsync()
    {
        var list = await _customerRepo.SearchAsync(CustomerSearchTerm, CustomerSearchTerm, default).ConfigureAwait(true);
        CustomerSearchResults = new ObservableCollection<Customer>(list);
    }

    [RelayCommand]
    private void SelectCustomer(Customer? customer)
    {
        if (customer != null)
        {
            SelectedCustomer = customer;
            IsCustomerSearchOpen = false;
        }
    }

    [RelayCommand]
    private void ClearCustomer() => SelectedCustomer = null;

    [RelayCommand]
    private void OpenNewCustomer() => IsNewCustomerOpen = true;

    [RelayCommand]
    private void CloseNewCustomer()
    {
        IsNewCustomerOpen = false;
        NewCustomerName = NewCustomerPhone = NewCustomerEmail = NewCustomerAddress = "";
    }

    [RelayCommand]
    private async Task SaveNewCustomerAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCustomerPhone)) return;
        var c = new Customer { Name = NewCustomerName, PhoneNumber = NewCustomerPhone.Trim(), Email = string.IsNullOrWhiteSpace(NewCustomerEmail) ? null : NewCustomerEmail, Address = string.IsNullOrWhiteSpace(NewCustomerAddress) ? null : NewCustomerAddress, Point = 0 };
        await _customerRepo.AddAsync(c, default).ConfigureAwait(true);
        SelectedCustomer = c;
        CloseNewCustomer();
    }

    [RelayCommand]
    private void OpenRedeemPoints()
    {
        if (SelectedCustomer != null)
        {
            var max = Math.Min(SelectedCustomer.Point, (int)((CurrentBill?.GrandTotal ?? 0) / _loyaltyValuePerPoint));
            PointsToRedeemText = max.ToString();
            IsRedeemPointsOpen = true;
        }
    }

    [RelayCommand]
    private void ConfirmRedeemPoints()
    {
        if (CurrentBill == null || SelectedCustomer == null) return;
        if (!int.TryParse(PointsToRedeemText, out var points) || points <= 0) return;
        points = Math.Min(points, SelectedCustomer.Point);
        var discount = points * _loyaltyValuePerPoint;
        CurrentBill.DiscountAmount += discount;
        CurrentBill.PointsRedeemed += points;
        CurrentBill.RecalcTotals();
        IsRedeemPointsOpen = false;
    }

    [RelayCommand]
    private void CloseRedeemPoints() => IsRedeemPointsOpen = false;
}
