using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.Generic;
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
    private readonly IDiscountRepository _discountRepo;
    private readonly IRecipeRepository _recipeRepo;
    private readonly IReceiptService _receiptService;
    private readonly IPrintService _printService;
    private readonly Services.IAuditLogger _auditLogger;
    private readonly Services.PendingTableService _pendingTable;
    private readonly IKitchenDisplayService _kitchenDisplay;
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
    [ObservableProperty] private bool _isPendingAdd;
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
    [ObservableProperty] private bool _isMergeConfirmOpen;
    [ObservableProperty] private BillViewModel? _mergeSourceBill;
    [ObservableProperty] private ObservableCollection<BillViewModel> _mergeBillCandidates = new();
    [ObservableProperty] private bool _isSplitPayment;
    [ObservableProperty] private ObservableCollection<PaymentEntryViewModel> _paymentEntries = new();
    [ObservableProperty] private string _selectedPaymentMethod = "Cash";
    [ObservableProperty] private decimal _paymentEntryAmount;
    [ObservableProperty] private string _tipAmountText = "";
    [ObservableProperty] private string _couponCode = "";
    [ObservableProperty] private bool _isReceiptPreviewOpen;
    [ObservableProperty] private string _receiptPreviewText = "";
    [ObservableProperty] private bool _isDrinkTagsPreviewOpen;
    [ObservableProperty] private ObservableCollection<Models.DrinkTagPreviewItem> _drinkTagPreviews = new();
    [ObservableProperty] private bool _isRecipeSidebarOpen;
    [ObservableProperty] private string _recipeSidebarTitle = "";
    [ObservableProperty] private ObservableCollection<string> _recipeSidebarIngredients = new();
    [ObservableProperty] private ObservableCollection<string> _recipeSidebarSteps = new();
    [ObservableProperty] private string _recipeSidebarOrderNote = "";
    [ObservableProperty] private string _paymentError = "";
    [ObservableProperty] private decimal _changeDue;

    public decimal SplitTotal => PaymentEntries.Sum(e => e.Amount);
    public bool ShowSingleAmount => !IsSplitPayment;
    public bool CanCompletePayment => CurrentBill != null && (
        IsSplitPayment && Math.Abs(SplitTotal - CurrentBill.GrandTotal) < 0.01m
        || !IsSplitPayment && PaymentAmount >= CurrentBill.GrandTotal - 0.01m);
    public string[] IceLevels { get; } = { "None", "Less", "Normal", "More" };
    public string[] ServiceTypes { get; } = { "Dine-in", "Takeaway", "Delivery" };
    public string[] PaymentMethods { get; } = { "Card", "Cash", "QR", "Other" };
    public string[] SugarLevels { get; } = { "None", "Less", "Normal", "More" };
    public string[] SizeOptions { get; } = { "S", "M", "L" };

    public BillViewModel? CurrentBill => CurrentBillIndex >= 0 && CurrentBillIndex < Bills.Count ? Bills[CurrentBillIndex] : null;
    public string CustomerDisplayText => SelectedCustomer == null ? "" : $"{SelectedCustomer.Name} ({SelectedCustomer.Point} pts)";
    public bool HasCustomer => SelectedCustomer != null;
    public string WaiterName => "Staff";
    public string CurrentBillOrderDisplay => CurrentBill == null ? "" : "Order " + CurrentBill.Label;

    public OrderSelectionViewModel(
        ICategoryRepository categoryRepo,
        IMenuItemRepository menuItemRepo,
        IOrderRepository orderRepo,
        IOrderDetailRepository detailRepo,
        IOrderDetailOptionRepository optionRepo,
        ICustomerRepository customerRepo,
        ITableRepository tableRepo,
        IDiscountRepository discountRepo,
        IRecipeRepository recipeRepo,
        IReceiptService receiptService,
        IPrintService printService,
        Services.IAuditLogger auditLogger,
        Services.PendingTableService pendingTable,
        IKitchenDisplayService kitchenDisplay,
        IConfiguration configuration)
    {
        _categoryRepo = categoryRepo;
        _menuItemRepo = menuItemRepo;
        _orderRepo = orderRepo;
        _detailRepo = detailRepo;
        _optionRepo = optionRepo;
        _customerRepo = customerRepo;
        _tableRepo = tableRepo;
        _discountRepo = discountRepo;
        _recipeRepo = recipeRepo;
        _receiptService = receiptService;
        _printService = printService;
        _auditLogger = auditLogger;
        _pendingTable = pendingTable;
        _kitchenDisplay = kitchenDisplay;
        _taxRate = decimal.TryParse(configuration["Tax:Rate"], out var tr) ? tr : 0.1m;
        _loyaltyEarnPerAmount = decimal.TryParse(configuration["Loyalty:EarnPerAmount"], out var ea) ? ea : 10000m;
        _loyaltyEarnPoints = int.TryParse(configuration["Loyalty:EarnPoints"], out var ep) ? ep : 1;
        _loyaltyValuePerPoint = decimal.TryParse(configuration["Loyalty:ValuePerPoint"], out var vp) ? vp : 100m;
        _ = LoadAsync();
        ApplyPendingTableIfAny();
        if (Bills.Count == 0) NewBill();
    }

    public void ApplyPendingTableIfAny()
    {
        var pending = _pendingTable.GetAndClear();
        if (pending != null)
            NewBillForTable(pending);
    }

    partial void OnSelectedCategoryChanged(Category? value) => FilterMenuItems();

    partial void OnSearchTermChanged(string value) => FilterMenuItems();
    partial void OnCurrentBillIndexChanged(int value)
    {
        OnPropertyChanged(nameof(CurrentBill));
        OpenReceiptPreviewCommand.NotifyCanExecuteChanged();
        OpenDrinkTagsPreviewCommand.NotifyCanExecuteChanged();
    }
    partial void OnPaymentAmountChanged(decimal value)
    {
        if (CurrentBill != null)
            ChangeDue = Math.Max(0, value - CurrentBill.GrandTotal);
        OnPropertyChanged(nameof(CanCompletePayment));
    }
    partial void OnIsSplitPaymentChanged(bool value) { OnPropertyChanged(nameof(CanCompletePayment)); OnPropertyChanged(nameof(ShowSingleAmount)); }
    partial void OnPaymentEntriesChanged(ObservableCollection<PaymentEntryViewModel> value) { OnPropertyChanged(nameof(SplitTotal)); OnPropertyChanged(nameof(CanCompletePayment)); }
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
            if (Categories.Count > 0 && SelectedCategory == null)
                SelectedCategory = Categories[0];
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
    private void SetPaymentMethod(string? method)
    {
        if (!string.IsNullOrEmpty(method)) SelectedPaymentMethod = method;
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
    private void SelectProductForCustomize(MenuItem? item)
    {
        if (item == null || CurrentBill == null) return;
        var line = new BillLineViewModel
        {
            MenuItemId = item.Id,
            ProductName = item.Name ?? "",
            Quantity = 1,
            UnitPrice = item.SellPrice,
            LineTotal = item.SellPrice,
            ParentBill = CurrentBill,
            IceLevel = "Normal",
            SugarLevel = "Normal",
            Size = "M"
        };
        CustomizationLine = line;
        IsPendingAdd = true;
        IsCustomizationOpen = true;
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
        OpenReceiptPreviewCommand.NotifyCanExecuteChanged();
        OpenDrinkTagsPreviewCommand.NotifyCanExecuteChanged();
    }

    private bool CanAddProduct(MenuItem? item) => item != null && CurrentBill != null;

    [RelayCommand]
    private void AddPendingLineToOrder()
    {
        if (CurrentBill == null || CustomizationLine == null || !IsPendingAdd) return;
        CurrentBill.Lines.Add(CustomizationLine);
        CurrentBill.RecalcTotals();
        OpenReceiptPreviewCommand.NotifyCanExecuteChanged();
        OpenDrinkTagsPreviewCommand.NotifyCanExecuteChanged();
        IsCustomizationOpen = false;
        CustomizationLine = null;
        IsPendingAdd = false;
    }

    [RelayCommand]
    private void RemoveLine(BillLineViewModel? line)
    {
        if (line == null || CurrentBill == null) return;
        CurrentBill.Lines.Remove(line);
        CurrentBill.RecalcTotals();
        OpenReceiptPreviewCommand.NotifyCanExecuteChanged();
        OpenDrinkTagsPreviewCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void OpenMergeDialog()
    {
        if (CurrentBill == null || Bills.Count < 2) return;
        MergeSourceBill = null;
        MergeBillCandidates = new ObservableCollection<BillViewModel>(Bills.Where(b => b != CurrentBill));
        IsMergeConfirmOpen = true;
    }

    [RelayCommand]
    private void RequestMergeBill(BillViewModel? source)
    {
        if (source == null || CurrentBill == null || source == CurrentBill || !Bills.Contains(source)) return;
        MergeSourceBill = source;
        IsMergeConfirmOpen = true;
    }

    [RelayCommand]
    private void ConfirmMergeBill()
    {
        if (MergeSourceBill == null || CurrentBill == null || MergeSourceBill == CurrentBill) { IsMergeConfirmOpen = false; MergeSourceBill = null; return; }
        var source = MergeSourceBill;
        var idx = Bills.IndexOf(source);
        foreach (var line in source.Lines.ToList())
        {
            var copy = new BillLineViewModel
            {
                MenuItemId = line.MenuItemId,
                ProductName = line.ProductName,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                IceLevel = line.IceLevel,
                SugarLevel = line.SugarLevel,
                Size = line.Size,
                Note = line.Note,
                IsCombo = line.IsCombo,
                ParentBill = CurrentBill
            };
            copy.RecalcTotal();
            CurrentBill.Lines.Add(copy);
        }
        CurrentBill.RecalcTotals();
        OpenReceiptPreviewCommand.NotifyCanExecuteChanged();
        OpenDrinkTagsPreviewCommand.NotifyCanExecuteChanged();
        Bills.Remove(source);
        if (CurrentBillIndex >= Bills.Count) CurrentBillIndex = Math.Max(0, Bills.Count - 1);
        IsMergeConfirmOpen = false;
        MergeSourceBill = null;
    }

    [RelayCommand]
    private void CancelMergeBill()
    {
        IsMergeConfirmOpen = false;
        MergeSourceBill = null;
    }

    [RelayCommand]
    private void CustomizeLine(BillLineViewModel? line)
    {
        if (line == null) return;
        CustomizationLine = line;
        IsPendingAdd = false;
        IsCustomizationOpen = true;
    }

    [RelayCommand]
    private void CloseCustomization()
    {
        IsCustomizationOpen = false;
        CustomizationLine = null;
        IsPendingAdd = false;
    }

    [RelayCommand]
    private void ConfirmCustomization()
    {
        if (IsPendingAdd)
            AddPendingLineToOrder();
        else
            CloseCustomization();
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
        ChangeDue = 0;
        PaymentError = "";
        PaymentEntries.Clear();
        IsSplitPayment = false;
        TipAmountText = "";
        IsPaymentOpen = true;
        OnPropertyChanged(nameof(CanCompletePayment));
    }

    [RelayCommand]
    private void AddPaymentEntry()
    {
        if (CurrentBill == null || PaymentEntryAmount <= 0) return;
        PaymentEntries.Add(new PaymentEntryViewModel { Method = SelectedPaymentMethod, Amount = PaymentEntryAmount });
        PaymentEntryAmount = 0;
        OnPropertyChanged(nameof(SplitTotal));
        OnPropertyChanged(nameof(CanCompletePayment));
    }

    [RelayCommand]
    private void RemovePaymentEntry(PaymentEntryViewModel? entry)
    {
        if (entry != null) PaymentEntries.Remove(entry);
        OnPropertyChanged(nameof(SplitTotal));
        OnPropertyChanged(nameof(CanCompletePayment));
    }

    [RelayCommand]
    private async Task ApplyCouponAsync()
    {
        if (CurrentBill == null || string.IsNullOrWhiteSpace(CouponCode)) return;
        var discounts = await _discountRepo.GetAllAsync(default).ConfigureAwait(true);
        var discount = discounts.FirstOrDefault(d => string.Equals((d.Name ?? "").Trim(), CouponCode.Trim(), StringComparison.OrdinalIgnoreCase) && d.IsActive);
        if (discount == null) return;
        CurrentBill.DiscountAmount = Math.Round(CurrentBill.Subtotal * (discount.Percentage / 100m), 2);
        CurrentBill.RecalcTotals();
        OnPropertyChanged(nameof(CanCompletePayment));
    }

    [RelayCommand(CanExecute = nameof(CanCompletePayment))]
    private async Task CompletePaymentAsync()
    {
        if (CurrentBill == null || !CanCompletePayment) return;
        PaymentError = "";
        var billToClear = CurrentBill;
        var success = false;
        try
        {
            var paymentNote = IsSplitPayment && PaymentEntries.Count > 0
                ? "Payment: " + string.Join(", ", PaymentEntries.Select(e => $"{e.Method} {e.Amount:N0}"))
                : null;
            var tip = decimal.TryParse(TipAmountText?.Replace(",", ""), out var t) ? t : 0m;
            var tipNote = tip > 0 ? " Tip: " + tip.ToString("N0") : "";
            var order = new Order
            {
                TotalPrice = CurrentBill.GrandTotal,
                Note = (paymentNote ?? "") + tipNote,
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
            var kitchenDisplay = await _kitchenDisplay.GetOrderDisplayAsync(orderId, default).ConfigureAwait(true);
            if (kitchenDisplay != null)
                _printService.PrintKitchenTicket(_kitchenDisplay.FormatKitchenOrderAsText(kitchenDisplay));

            var payMethod = IsSplitPayment ? "Split" : SelectedPaymentMethod;
            var receiptData = await _receiptService.BuildReceiptFromOrderAsync(orderId, payMethod, default).ConfigureAwait(true);
            var receiptText = _receiptService.FormatReceiptAsText(receiptData);
            _printService.PrintReceipt(receiptText);
            if (string.Equals(SelectedPaymentMethod, "Cash", StringComparison.OrdinalIgnoreCase) || payMethod == "Split")
                _printService.OpenCashDrawer();
            success = true;
        }
        catch (Exception ex)
        {
            PaymentError = ex.Message;
        }

        if (!success) return;

        Bills.Remove(billToClear);
        if (CurrentBillIndex >= Bills.Count) CurrentBillIndex = Math.Max(0, Bills.Count - 1);
        if (Bills.Count == 0) NewBill();
        IsPaymentOpen = false;
        PaymentAmount = 0;
        ChangeDue = 0;
        TipAmountText = "";
        PaymentEntries.Clear();
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
        OpenReceiptPreviewCommand.NotifyCanExecuteChanged();
        OpenDrinkTagsPreviewCommand.NotifyCanExecuteChanged();
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

    private bool CanOpenReceiptPreview() => CurrentBill != null && CurrentBill.Lines.Count > 0;

    [RelayCommand(CanExecute = nameof(CanOpenReceiptPreview))]
    private void OpenReceiptPreview()
    {
        if (CurrentBill == null) return;
        var data = _receiptService.BuildReceiptFromBill(CurrentBill);
        ReceiptPreviewText = _receiptService.FormatReceiptAsText(data);
        IsReceiptPreviewOpen = true;
    }

    [RelayCommand]
    private void PrintReceiptFromPreview()
    {
        _printService.PrintReceipt(ReceiptPreviewText);
        IsReceiptPreviewOpen = false;
    }

    [RelayCommand]
    private void CloseReceiptPreview()
    {
        IsReceiptPreviewOpen = false;
    }

    private bool CanOpenDrinkTagsPreview() => CurrentBill != null && CurrentBill.Lines.Count > 0;

    [RelayCommand(CanExecute = nameof(CanOpenDrinkTagsPreview))]
    private void OpenDrinkTagsPreview()
    {
        if (CurrentBill == null) return;
        var list = new List<Models.DrinkTagPreviewItem>();
        foreach (var line in CurrentBill.Lines)
        {
            for (var i = 0; i < line.Quantity; i++)
            {
                list.Add(new Models.DrinkTagPreviewItem
                {
                    ProductName = line.ProductName,
                    Size = line.Size,
                    Detail = line.CustomizationSummary,
                    Note = line.Note
                });
            }
        }
        DrinkTagPreviews = new ObservableCollection<Models.DrinkTagPreviewItem>(list);
        IsDrinkTagsPreviewOpen = true;
    }

    [RelayCommand]
    private void PrintDrinkTags()
    {
        foreach (var tag in DrinkTagPreviews)
            _printService.PrintDrinkTag(tag.ToTagText());
        IsDrinkTagsPreviewOpen = false;
    }

    [RelayCommand]
    private void CloseDrinkTagsPreview()
    {
        IsDrinkTagsPreviewOpen = false;
    }

    [RelayCommand]
    private async Task OpenRecipeForLineAsync(BillLineViewModel? line)
    {
        if (line == null) return;
        RecipeSidebarTitle = line.ProductName;
        RecipeSidebarOrderNote = line.Note ?? "";
        var recipe = await _recipeRepo.GetByMenuItemAndNoteAsync(line.MenuItemId, line.Note, default).ConfigureAwait(true);
        SetRecipeSidebarContent(recipe);
        IsRecipeSidebarOpen = true;
    }

    private void SetRecipeSidebarContent(DrinkRecipe? recipe)
    {
        RecipeSidebarIngredients = recipe != null ? new ObservableCollection<string>(recipe.Ingredients) : new ObservableCollection<string>();
        RecipeSidebarSteps = recipe != null ? new ObservableCollection<string>(recipe.Steps) : new ObservableCollection<string>();
    }

    [RelayCommand]
    private void CloseRecipeSidebar()
    {
        IsRecipeSidebarOpen = false;
    }

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
