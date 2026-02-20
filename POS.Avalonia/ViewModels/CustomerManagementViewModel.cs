using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Core.Contracts;
using POS.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class CustomerManagementViewModel : ViewModelBase
{
    private readonly ICustomerRepository _customerRepo;
    private readonly IOrderRepository _orderRepo;

    [ObservableProperty] private ObservableCollection<Customer> _customers = new();
    [ObservableProperty] private Customer? _selectedCustomer;
    [ObservableProperty] private ObservableCollection<Order> _customerOrders = new();
    [ObservableProperty] private string _searchTerm = "";
    [ObservableProperty] private bool _isLoading;
    public bool HasSelectedCustomer => SelectedCustomer != null;

    public CustomerManagementViewModel(ICustomerRepository customerRepo, IOrderRepository orderRepo)
    {
        _customerRepo = customerRepo;
        _orderRepo = orderRepo;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _customerRepo.GetAllAsync(default).ConfigureAwait(true);
            Customers = new ObservableCollection<Customer>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _customerRepo.SearchAsync(SearchTerm, SearchTerm, default).ConfigureAwait(true);
            Customers = new ObservableCollection<Customer>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedCustomerChanged(Customer? value)
    {
        OnPropertyChanged(nameof(HasSelectedCustomer));
        if (value == null)
        {
            CustomerOrders = new ObservableCollection<Order>();
            return;
        }
        _ = LoadCustomerOrdersAsync(value.Id);
    }

    private async Task LoadCustomerOrdersAsync(int customerId)
    {
        var all = await _orderRepo.GetAllAsync(default).ConfigureAwait(true);
        var filtered = all.Where(o => o.CustomerId == customerId).ToList();
        CustomerOrders = new ObservableCollection<Order>(filtered);
    }
}
