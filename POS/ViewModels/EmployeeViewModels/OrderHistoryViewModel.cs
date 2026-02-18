using CommunityToolkit.Mvvm.ComponentModel;
using POS.Models;
using POS.Services.Dao;
using System.Collections.ObjectModel;

namespace POS.ViewModels.EmployeeViewModels
{
    public partial class OrderHistoryViewModel : ObservableRecipient
    {
        private readonly IDao _dao;
        public ObservableCollection<Order> AllOrders { get; private set; } = new();
        public ObservableCollection<Order> Orders { get; private set; } = new();
        public ObservableCollection<Order> FilteredOrders { get; private set; } = new();
        public Order SelectedOrder;
        public List<Order> SelectedOrders = new();
        private string _searchTerm;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private const int ItemsPerPage = 10;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    FilterOrders();
                }
            }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    FilterOrders();
                }
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    FilterOrders();
                }
            }
        }

        public OrderHistoryViewModel(IDao dao)
        {
            _dao = dao;
            LoadOrders(1, ItemsPerPage);
            FilterOrders();
            Orders.CollectionChanged += (sender, args) => FilterOrders();
        }

        public void LoadOrders(int page, int itemsPerPage)
        {
            AllOrders = new ObservableCollection<Order>(_dao.GetOrders());
            Orders = new ObservableCollection<Order>(AllOrders.Skip((page - 1) * itemsPerPage).Take(itemsPerPage));
        }

        public List<Order> GetOrders()
        {
            return _dao.GetOrders();
        }

        public async Task AddOrder(Order order)
        {
            await _dao.AddOrderAsync(order);
            Orders.Add(order);
        }

        public async Task EditOrder(Order order, string newNote, string newPhoneNumber, DateTime newCreatedAt)
        {
            Order newOrder = new()
            {
                Id = order.Id,
                Note = newNote,
                PhoneNumber = newPhoneNumber,
                CreatedAt = newCreatedAt
            };
            await _dao.EditOrderAsync(newOrder);

            order.Note = newNote;
            order.PhoneNumber = newPhoneNumber;
            order.CreatedAt = newCreatedAt;
            FilterOrders();
        }

        public async Task DeleteOrder(Order order)
        {
            await _dao.DeleteOrderAsync(order.Id);
            AllOrders.Remove(order);
            FilterOrders();
        }

        public async Task DeleteOrders(List<Order> orders)
        {
            foreach (var selectedOrder in orders)
            {
                await DeleteOrder(selectedOrder);
                await Task.Delay(100);
            }
            FilterOrders();
        }

        private void FilterOrders()
        {
            var filteredItems = AllOrders.ToList();

            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                filteredItems = filteredItems
                    .Where(item => item.Id.ToString().Contains(_searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (_startDate.HasValue && _endDate.HasValue)
            {
                filteredItems = filteredItems
                    .Where(item => item.CreatedAt >= _startDate.Value && item.CreatedAt <= _endDate.Value)
                    .ToList();
            }

            UpdateFilteredOrders(filteredItems);
        }

        public void FilterOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        private void UpdateFilteredOrders(List<Order> items)
        {
            FilteredOrders.Clear();

            foreach (var item in items)
            {
                FilteredOrders.Add(item);
            }
        }
    }
}
