using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class BankTransactionViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<BankTransactionItem> _transactions = new();
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private decimal _totalCredits;
    [ObservableProperty] private decimal _totalDebits;

    public BankTransactionViewModel() => LoadCommand.ExecuteAsync(null);

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        await Task.Delay(300);
        Transactions = new ObservableCollection<BankTransactionItem>
        {
            new("Payment - Order #1042", 85000, "Credit", "TXN001", DateTime.Today),
            new("Payment - Order #1041", 125000, "Credit", "TXN002", DateTime.Today),
            new("Supplier - Bean Co.", -450000, "Debit", "TXN003", DateTime.Today.AddDays(-1)),
            new("Payment - Order #1040", 65000, "Credit", "TXN004", DateTime.Today.AddDays(-1)),
            new("Utility Bill", -280000, "Debit", "TXN005", DateTime.Today.AddDays(-2)),
            new("Payment - Order #1039", 95000, "Credit", "TXN006", DateTime.Today.AddDays(-2)),
        };
        TotalCredits = Transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
        TotalDebits = Transactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
        IsLoading = false;
    }
}

public class BankTransactionItem
{
    public string Description { get; }
    public decimal Amount { get; }
    public string Type { get; }
    public string Reference { get; }
    public DateTime Date { get; }
    public bool IsCredit => Amount > 0;

    public BankTransactionItem(string desc, decimal amount, string type, string reference, DateTime date)
    {
        Description = desc;
        Amount = amount;
        Type = type;
        Reference = reference;
        Date = date;
    }
}
