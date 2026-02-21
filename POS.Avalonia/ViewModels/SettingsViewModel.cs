using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsStore _store;
    private readonly IPrintService _printService;

    public static IReadOnlyList<string> UiSizeOptions => UiSizePresets.AllKeys;

    [ObservableProperty] private string _selectedUiSize = UiSizePresets.Key1080;

    [ObservableProperty] private string _businessName = "";
    [ObservableProperty] private string _businessAddress = "";
    [ObservableProperty] private string _businessTaxId = "";
    [ObservableProperty] private string _receiptThankYou = "";
    [ObservableProperty] private string _taxRate = "0.1";
    [ObservableProperty] private string _taxName = "VAT";
    [ObservableProperty] private string _dbHost = "localhost";
    [ObservableProperty] private string _dbPort = "5432";
    [ObservableProperty] private string _dbUser = "postgres";
    [ObservableProperty] private string _dbPassword = "";
    [ObservableProperty] private string _dbName = "dev_meo_cf";
    [ObservableProperty] private string _printerReceipt = "";
    [ObservableProperty] private string _printerKitchen = "";
    [ObservableProperty] private string _paymentMethods = "Cash, Card";
    [ObservableProperty] private bool _saveSuccess;
    [ObservableProperty] private bool _isSaving;
    public bool CanSave => !IsSaving;

    public string Title => "Settings";

    public SettingsViewModel(ISettingsStore store, IPrintService printService)
    {
        _store = store;
        _printService = printService;
        Load();
    }

    private void Load()
    {
        BusinessName = _store.Get("Receipt:BusinessName") ?? "MEOCAFE";
        BusinessAddress = _store.Get("Receipt:Address") ?? "";
        BusinessTaxId = _store.Get("Receipt:TaxId") ?? "";
        ReceiptThankYou = _store.Get("Receipt:ThankYou") ?? "Thank you!";
        TaxRate = _store.Get("Tax:Rate") ?? "0.1";
        TaxName = _store.Get("Tax:Name") ?? "VAT";
        DbHost = _store.Get("DB_HOST") ?? "localhost";
        DbPort = _store.Get("DB_PORT") ?? "5432";
        DbUser = _store.Get("DB_USER") ?? "postgres";
        DbPassword = _store.Get("DB_PASSWORD") ?? "";
        DbName = _store.Get("DB_NAME") ?? "dev_meo_cf";
        PrinterReceipt = _store.Get("Printer:Receipt") ?? "";
        PrinterKitchen = _store.Get("Printer:Kitchen") ?? "";
        PaymentMethods = _store.Get("PaymentMethods:Names") ?? "Cash, Card";
        SelectedUiSize = _store.Get("UI:Size") ?? UiSizePresets.Key1080;
    }

    partial void OnBusinessNameChanged(string value) => _store.Set("Receipt:BusinessName", value);
    partial void OnBusinessAddressChanged(string value) => _store.Set("Receipt:Address", value);
    partial void OnBusinessTaxIdChanged(string value) => _store.Set("Receipt:TaxId", value);
    partial void OnReceiptThankYouChanged(string value) => _store.Set("Receipt:ThankYou", value);
    partial void OnTaxRateChanged(string value) => _store.Set("Tax:Rate", value);
    partial void OnTaxNameChanged(string value) => _store.Set("Tax:Name", value);
    partial void OnDbHostChanged(string value) => _store.Set("DB_HOST", value);
    partial void OnDbPortChanged(string value) => _store.Set("DB_PORT", value);
    partial void OnDbUserChanged(string value) => _store.Set("DB_USER", value);
    partial void OnDbPasswordChanged(string value) => _store.Set("DB_PASSWORD", value);
    partial void OnDbNameChanged(string value) => _store.Set("DB_NAME", value);
    partial void OnPrinterReceiptChanged(string value) => _store.Set("Printer:Receipt", value);
    partial void OnPrinterKitchenChanged(string value) => _store.Set("Printer:Kitchen", value);
    partial void OnPaymentMethodsChanged(string value) => _store.Set("PaymentMethods:Names", value);
    partial void OnSelectedUiSizeChanged(string value) => _store.Set("UI:Size", value);

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
            _store.Set("DB_HOST", DbHost);
            _store.Set("DB_PORT", DbPort);
            _store.Set("DB_USER", DbUser);
            _store.Set("DB_PASSWORD", DbPassword);
            _store.Set("DB_NAME", DbName);
            _store.Set("Printer:Receipt", PrinterReceipt);
            _store.Set("Printer:Kitchen", PrinterKitchen);
            _store.Set("PaymentMethods:Names", PaymentMethods);
            _store.Set("UI:Size", SelectedUiSize);
            await _store.SaveAsync(default).ConfigureAwait(true);
            SaveSuccess = true;
            var (w, h) = UiSizePresets.GetDimensions(SelectedUiSize);
            Dispatcher.UIThread.Post(() =>
            {
                UiSizePresetValues.ApplyToApplication(SelectedUiSize);
                Program.ApplyDisplaySize(w, h);
            });
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
    private void TestPrint() => _printService.PrintReceipt("Test receipt\n\n");
}
