using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using POS.Avalonia.Localization;
using POS.Avalonia.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsStore _store;
    private readonly IPrintService _printService;
    private readonly AppWindowDebugInfo _debugInfo;
    private readonly IDatabaseSyncService _databaseSync;
    private readonly CurrentUserService _currentUser;
    private readonly IConfiguration _configuration;
    private readonly IStringLocalizer<StringsResource> _loc;

    public static IReadOnlyList<string> UiSizeOptions => UiSizePresets.AllKeys;
    public static IReadOnlyList<string> ThemeOptions { get; } = new[] { ThemeApplier.KeyDark, ThemeApplier.KeyLight };
    public AppWindowDebugInfo DebugInfo => _debugInfo;

    public ObservableCollection<string> PrinterOptions { get; } = new()
        { "", "192.168.1.50:9100", "localhost:9100" };

    public IReadOnlyList<string> SettingsSections { get; }

    [ObservableProperty] private string _selectedSettingsSection = "";

    partial void OnSelectedSettingsSectionChanged(string value)
    {
        OnPropertyChanged(nameof(IsDisplaySectionVisible));
        OnPropertyChanged(nameof(IsBusinessSectionVisible));
        OnPropertyChanged(nameof(IsOperationalSectionVisible));
        OnPropertyChanged(nameof(IsPrinterSectionVisible));
        OnPropertyChanged(nameof(IsReceiptSectionVisible));
        OnPropertyChanged(nameof(IsPaymentSectionVisible));
        OnPropertyChanged(nameof(IsDatabaseSectionVisible));
    }

    [ObservableProperty] private string _selectedUiSize = UiSizePresets.Key1080;
    [ObservableProperty] private string _selectedTheme = ThemeApplier.KeyDark;
    [ObservableProperty] private bool _autoPrintReceipt;
    [ObservableProperty] private bool _soundOnOrder;
    [ObservableProperty] private string _businessName = "";
    [ObservableProperty] private string _businessAddress = "";
    [ObservableProperty] private string _businessTaxId = "";
    [ObservableProperty] private string _receiptThankYou = "";
    [ObservableProperty] private string _taxRate = "0.1";
    [ObservableProperty] private string _taxName = "VAT";
    [ObservableProperty] private string _databasePath = "";
    [ObservableProperty] private string _staffDisplayName = "";
    [ObservableProperty] private string _printerReceipt = "";
    [ObservableProperty] private string _printerKitchen = "";
    [ObservableProperty] private string _receiptHeaderIconPath = "";
    [ObservableProperty] private string _paymentMethods = "Cash, Card";
    [ObservableProperty] private bool _saveSuccess;
    [ObservableProperty] private bool _isSaving;
    [ObservableProperty] private string _gitSyncMessage = "";

    public bool CanSave => !IsSaving;
    public string Title => _loc["Settings_Title"].Value ?? "Cài đặt";

    private string L(string key, string fallback) => _loc[key].Value ?? fallback;

    public bool IsDisplaySectionVisible => SelectedSettingsSection == L("Settings_Section_Display", "Display");
    public bool IsBusinessSectionVisible => SelectedSettingsSection == L("Settings_Section_Business", "Business");
    public bool IsOperationalSectionVisible => SelectedSettingsSection == L("Settings_Section_Operational", "Operational");
    public bool IsPrinterSectionVisible => SelectedSettingsSection == L("Settings_Section_Printer", "Printer");
    public bool IsReceiptSectionVisible => SelectedSettingsSection == L("Settings_Section_Receipt", "Receipt");
    public bool IsPaymentSectionVisible => SelectedSettingsSection == L("Settings_Section_Payment", "Payment");
    public bool IsDatabaseSectionVisible => SelectedSettingsSection == L("Settings_Section_Database", "Database");

    public SettingsViewModel(
        ISettingsStore store,
        IPrintService printService,
        AppWindowDebugInfo debugInfo,
        IDatabaseSyncService databaseSync,
        CurrentUserService currentUser,
        IConfiguration configuration,
        IStringLocalizer<StringsResource> loc)
    {
        _store = store;
        _printService = printService;
        _debugInfo = debugInfo;
        _databaseSync = databaseSync;
        _currentUser = currentUser;
        _configuration = configuration;
        _loc = loc;
        SettingsSections = new[]
        {
            L("Settings_Section_Display", "Display"),
            L("Settings_Section_Business", "Business"),
            L("Settings_Section_Operational", "Operational"),
            L("Settings_Section_Printer", "Printer"),
            L("Settings_Section_Receipt", "Receipt"),
            L("Settings_Section_Payment", "Payment"),
            L("Settings_Section_Database", "Database")
        };
        _selectedSettingsSection = SettingsSections[0];
        Load();
    }

    private void Load()
    {
        BusinessName = _store.Get("Receipt:BusinessName") ?? "Hien Xanh";
        BusinessAddress = _store.Get("Receipt:Address") ?? "";
        BusinessTaxId = _store.Get("Receipt:TaxId") ?? "";
        ReceiptThankYou = _store.Get("Receipt:ThankYou") ?? "Cảm ơn quý khách!";
        TaxRate = _store.Get("Tax:Rate") ?? "0.1";
        TaxName = _store.Get("Tax:Name") ?? "VAT";
        DatabasePath = _store.Get("Database:Path") ?? _configuration["Database:Path"] ?? "data/hienxanh.db";
        StaffDisplayName = _store.Get("Staff:DisplayName") ?? _configuration["Staff:DisplayName"] ?? "HienXanh";
        PrinterReceipt = _store.Get("Printer:Receipt") ?? "";
        PrinterKitchen = _store.Get("Printer:Kitchen") ?? "";
        ReceiptHeaderIconPath = _store.Get("Receipt:HeaderIconPath") ?? "";
        PaymentMethods = _store.Get("PaymentMethods:Names") ?? "Cash, Card";
        SelectedUiSize = _store.Get("UI:Size") ?? UiSizePresets.Key1080;
        SelectedTheme = _store.Get("UI:Theme") ?? ThemeApplier.KeyDark;
        AutoPrintReceipt = _store.Get("Operational:AutoPrintReceipt") == "1";
        SoundOnOrder = _store.Get("Operational:SoundOnOrder") != "0";
        EnsurePrinterOptions();
    }

    private void EnsurePrinterOptions()
    {
        if (!string.IsNullOrEmpty(PrinterReceipt) && !PrinterOptions.Contains(PrinterReceipt))
            PrinterOptions.Add(PrinterReceipt);
        if (!string.IsNullOrEmpty(PrinterKitchen) && !PrinterOptions.Contains(PrinterKitchen))
            PrinterOptions.Add(PrinterKitchen);
    }

    partial void OnBusinessNameChanged(string value) => _store.Set("Receipt:BusinessName", value);
    partial void OnBusinessAddressChanged(string value) => _store.Set("Receipt:Address", value);
    partial void OnBusinessTaxIdChanged(string value) => _store.Set("Receipt:TaxId", value);
    partial void OnReceiptThankYouChanged(string value) => _store.Set("Receipt:ThankYou", value);
    partial void OnTaxRateChanged(string value) => _store.Set("Tax:Rate", value);
    partial void OnTaxNameChanged(string value) => _store.Set("Tax:Name", value);
    partial void OnDatabasePathChanged(string value) => _store.Set("Database:Path", value);
    partial void OnStaffDisplayNameChanged(string value) => _store.Set("Staff:DisplayName", value);
    partial void OnPrinterReceiptChanged(string value) => _store.Set("Printer:Receipt", value);
    partial void OnPrinterKitchenChanged(string value) => _store.Set("Printer:Kitchen", value);
    partial void OnPaymentMethodsChanged(string value) => _store.Set("PaymentMethods:Names", value);
    partial void OnReceiptHeaderIconPathChanged(string value) => _store.Set("Receipt:HeaderIconPath", value);
    partial void OnAutoPrintReceiptChanged(bool value) => _store.Set("Operational:AutoPrintReceipt", value ? "1" : "0");
    partial void OnSoundOnOrderChanged(bool value) => _store.Set("Operational:SoundOnOrder", value ? "1" : "0");

    [RelayCommand]
    private void ApplyTheme()
    {
        _store.Set("UI:Theme", SelectedTheme);
        _ = _store.SaveAsync(default);
        Dispatcher.UIThread.Post(() => ThemeApplier.Apply(SelectedTheme));
    }

    [RelayCommand]
    private void ApplyDisplaySize()
    {
        _store.Set("UI:Size", SelectedUiSize);
        _ = _store.SaveAsync(default);
        Dispatcher.UIThread.Post(() =>
        {
            UiSizePresetValues.ApplyToApplication(SelectedUiSize);
            if (UiSizePresets.IsFullScreen(SelectedUiSize))
                Program.ApplyFullScreen();
            else
            {
                var (w, h) = UiSizePresets.GetDimensions(SelectedUiSize);
                Program.ApplyDisplaySize(w, h);
            }
        });
    }

    [RelayCommand]
    private void CancelDisplaySize() => SelectedUiSize = _store.Get("UI:Size") ?? UiSizePresets.Key1080;

    [RelayCommand]
    private async Task SaveAsync()
    {
        IsSaving = true;
        SaveSuccess = false;
        try
        {
            _store.Set("Receipt:BusinessName", BusinessName);
            _store.Set("Receipt:Address", BusinessAddress);
            _store.Set("Receipt:TaxId", BusinessTaxId);
            _store.Set("Receipt:ThankYou", ReceiptThankYou);
            _store.Set("Tax:Rate", TaxRate);
            _store.Set("Tax:Name", TaxName);
            _store.Set("Database:Path", DatabasePath);
            _store.Set("Staff:DisplayName", StaffDisplayName);
            _store.Set("Printer:Receipt", PrinterReceipt);
            _store.Set("Printer:Kitchen", PrinterKitchen);
            _store.Set("Receipt:HeaderIconPath", ReceiptHeaderIconPath);
            _store.Set("Operational:AutoPrintReceipt", AutoPrintReceipt ? "1" : "0");
            _store.Set("Operational:SoundOnOrder", SoundOnOrder ? "1" : "0");
            _store.Set("PaymentMethods:Names", PaymentMethods);
            await _store.SaveAsync(default).ConfigureAwait(true);
            _currentUser.RefreshDisplayName(_store, _configuration);
            SaveSuccess = true;
        }
        finally
        {
            IsSaving = false;
            OnPropertyChanged(nameof(CanSave));
        }
    }

    [RelayCommand]
    private void Refresh() => Load();

    [RelayCommand]
    private void TestPrint() => _printService.PrintReceipt("Hóa đơn thử\n\n");

    [RelayCommand]
    private void PrepareGitSync()
    {
        GitSyncMessage = "";
        try
        {
            _databaseSync.CheckpointForGitSync();
            GitSyncMessage = _loc["Settings_GitSyncDone"].Value ?? "";
        }
        catch (System.Exception ex)
        {
            GitSyncMessage = ex.Message;
        }
    }
}
