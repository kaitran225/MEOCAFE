using CommunityToolkit.Mvvm.ComponentModel;
using OpenTK.Graphics.OpenGL;
using POS.Models;
using POS.Services.Dao;
using POS.Services.OrderDao;
using POS.Services.OrderDetailDao;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace POS.ViewModels.EmployeeViewModels
{
    public class OrderSelectionViewModel : ObservableObject
    {
        private readonly IDao _dao;
        private readonly IDao _postgresDao = new PostgreSqlDao();
        private string _searchTerm = "";
        private string _currentFilter = "All";
        private bool _isPaneOpen;

        private Category _selectedCategory;
        private OrderMenuItem _selectedMenuItem;
        public ObservableCollection<OrderItem> OrderItems { get; set; } = new();
        public ObservableCollection<ComboMenuItem> ComboMenuItems { get; set; } = new();
        public bool IsOrderItemsNotEmpty => OrderItems.Count > 0;

        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<Item> MenuItems { get; set; } = new();
        public ObservableCollection<Item> Combo { get; set; } = new();
        public ObservableCollection<Item> FilteredMenuItems { get; set; } = new();

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    FilterMenuItems();
                }
            }
        }

        public string CurrentFilter
        {
            get => _currentFilter;
            set => SetProperty(ref _currentFilter, value);
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public OrderMenuItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set => SetProperty(ref _selectedMenuItem, value);
        }

        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => SetProperty(ref _isPaneOpen, value);
        }

        private decimal _totalPrice = 0;
        public decimal TotalPrice
        {
            get => _totalPrice;
            set => SetProperty(ref _totalPrice, value);
        }

        private decimal _grandPrice = 0;
        public decimal GrandPrice
        {
            get => _grandPrice;
            set => SetProperty(ref _grandPrice, value);
        }

        private decimal _discount = 0;
        public decimal Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value);
        }

        private readonly Dictionary<int, int> _menuItemQuantities = new();

        private readonly OrderDao _orderDao;
        private readonly OrderDetailDao _orderDetailDao;

        public OrderSelectionViewModel(IDao dao)
        {
            _dao = dao;
            _orderDao = new OrderDao();
            _orderDetailDao = new OrderDetailDao();
            LoadCategories();
            LoadMenuItems();
            LoadComboItems();
        }

        private void LoadCategories()
        {
            var tempCategories = _dao.GetCategories();
            foreach (var category in tempCategories)
            {
                category.MenuItems = _dao.GetMenuItems().Where(m => m.CategoryId == category.Id).ToList();
            }
            Categories = new ObservableCollection<Category>(tempCategories);
        }

        private async void LoadMenuItems()
        {
            var menuItems = _dao.GetMenuItems();
            var tasks = menuItems.Select(async item =>
            {
                var orderMenuItem = new OrderMenuItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    SellPrice = item.SellPrice,
                    Quantity = item.Quantity,
                    Image = item.Image,
                    CapitalPrice = item.CapitalPrice,
                    IsPlaceholder = item.IsPlaceholder,
                    CategoryId = item.CategoryId,
                    Category = item.Category,
                    Discount = item.Discount,
                    QuantitySelected = 0,
                    IsCombo = false
                };

                var itemViewModel = new ItemViewModel(orderMenuItem);
                await itemViewModel.LoadImageAsync();
                orderMenuItem.ItemImage = itemViewModel.Image;
                orderMenuItem.Discount = _dao.GetDiscount(orderMenuItem.Id);
                if (orderMenuItem.Discount != null)
                {
                    if (!orderMenuItem.Discount.IsDisabled)
                        orderMenuItem.DiscountedPrice = orderMenuItem.SellPrice - orderMenuItem.Discount.Percentage * orderMenuItem.SellPrice / 100;
                    else
                        orderMenuItem.DiscountedPrice = null;
                } 
                else
                {
                    orderMenuItem.DiscountedPrice = null;
                }

                return orderMenuItem;
            });

            var orderMenuItems = await Task.WhenAll(tasks);
            foreach (var orderMenuItem in orderMenuItems)
            {
                MenuItems.Add(orderMenuItem);
            }

            // ============= Load Combo =================

            var comboMenuItems = _dao.GetComboMenuItems();
            tasks = comboMenuItems.Select(async item =>
            {
                var comboMenuItem = new OrderMenuItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    SellPrice = item.SellPrice,
                    Quantity = item.Quantity,
                    Image = item.Image,
                    IsCombo = true
                };
                var itemViewModel = new ItemViewModel(comboMenuItem);
                await itemViewModel.LoadImageAsync();
                comboMenuItem.ItemImage = itemViewModel.Image;
                return comboMenuItem;
            });

            var _comboMenuItems = await Task.WhenAll(tasks);
            foreach (var item in _comboMenuItems)
            {
                MenuItems.Add(item);
            }

            ApplyFilter(_currentFilter);
        }

        private void LoadComboItems()
        {
            var comboMenuItems = _dao.GetComboMenuItems();
            foreach (var comboMenuItem in comboMenuItems)
            {
                ComboMenuItems.Add(comboMenuItem);
            }
        }

        private void FilterMenuItems()
        {
            var filteredItems = ApplyCategoryFilter(_currentFilter).OfType<OrderMenuItem>();
            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                filteredItems = filteredItems
                    .Where(item => item.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            UpdateFilteredMenuItems(filteredItems);
        }

        private IEnumerable<Item> ApplyCategoryFilter(string filterOption)
        {
            return filterOption switch
            {
                "All" => MenuItems,
                "Combo" => MenuItems.Where(item => item is OrderMenuItem orderMenuItem && orderMenuItem?.IsCombo == true),
                _ => MenuItems.Where(item => item is OrderMenuItem orderMenuItem && orderMenuItem.CategoryId == _categories.FirstOrDefault(c => c.Name == filterOption)?.Id)
            };
        }

        public void ApplyFilter(string filterOption)
        {
            _currentFilter = filterOption;
            FilterMenuItems();
        }

        private void UpdateFilteredMenuItems(IEnumerable<Item> items)
        {
            FilteredMenuItems.Clear();
            foreach (var item in items)
            {
                FilteredMenuItems.Add(item);
            }
        }

        public void AddItemToOrder(OrderMenuItem menuItem)
        {
            UpdateMenuItemQuantity(menuItem, 1);
            var orderItem = OrderItems.FirstOrDefault(item => item.MenuItem.Id == menuItem.Id && item.IsCombo == menuItem.IsCombo);
            if (orderItem != null)
            {
                orderItem.Quantity++;
                orderItem.Price += menuItem.SellPrice;
                menuItem.QuantitySelected = orderItem.Quantity;
            }
            else
            {
                OrderItems.Add(new OrderItem { MenuItem = menuItem, Quantity = 1, Price = menuItem.SellPrice, IsCombo = (menuItem?.IsCombo == true) });
                menuItem.QuantitySelected = 1;
            }
            UpdatePrices(menuItem.SellPrice, menuItem.SellPrice - menuItem?.DiscountedPrice ?? 0);
            OnPropertyChanged(nameof(IsOrderItemsNotEmpty));
        }

        public void RemoveItemFromOrder(OrderMenuItem menuItem)
        {
            var orderItem = OrderItems.FirstOrDefault(item => item.MenuItem.Id == menuItem.Id && item.IsCombo == menuItem.IsCombo);
            if (orderItem != null)
            {
                orderItem.Quantity--;
                orderItem.Price -= menuItem.SellPrice;
                menuItem.QuantitySelected = orderItem.Quantity;
                if (orderItem.Quantity == 0)
                {
                    OrderItems.Remove(orderItem);
                }
                UpdatePrices(-menuItem.SellPrice, menuItem.SellPrice + menuItem?.DiscountedPrice ?? 0);
            }
            OnPropertyChanged(nameof(IsOrderItemsNotEmpty));
        }

        internal void AddItemToOrder(OrderItem orderItem)
        {
            var menuItem = MenuItems.OfType<OrderMenuItem>().FirstOrDefault(item => item.Id == orderItem.MenuItem.Id && item.IsCombo == orderItem.MenuItem.IsCombo);
            UpdateMenuItemQuantity(orderItem.MenuItem, 1);
            orderItem.Quantity++;
            menuItem.QuantitySelected = orderItem.Quantity;
            orderItem.Price += orderItem.MenuItem.SellPrice;
            UpdatePrices(orderItem.MenuItem.SellPrice, menuItem.SellPrice - menuItem?.DiscountedPrice ?? 0);
            OnPropertyChanged(nameof(IsOrderItemsNotEmpty));
        }

        internal void RemoveItemFromOrder(OrderItem orderItem)
        {
            var menuItem = MenuItems.OfType<OrderMenuItem>().FirstOrDefault(orderMenuItem => orderMenuItem.Id == orderItem.MenuItem.Id && orderMenuItem.IsCombo == orderItem.MenuItem.IsCombo);
            if (orderItem.Quantity > 0)
            {
                UpdateMenuItemQuantity(orderItem.MenuItem, -1);
                orderItem.Quantity--;
                menuItem.QuantitySelected = orderItem.Quantity;
                orderItem.Price -= orderItem.MenuItem.SellPrice;
                if (orderItem.Quantity == 0)
                {
                    OrderItems.Remove(orderItem);
                }
                UpdatePrices(-orderItem.MenuItem.SellPrice, menuItem.SellPrice + menuItem?.DiscountedPrice ?? 0);
                OnPropertyChanged(nameof(IsOrderItemsNotEmpty));
            }
        }

        private void UpdateMenuItemQuantity(OrderMenuItem menuItem, int change)
        {
            if (_menuItemQuantities.ContainsKey(menuItem.Id))
            {
                _menuItemQuantities[menuItem.Id] += change;
                menuItem.QuantitySelected = _menuItemQuantities[menuItem.Id];
            }
            else
            {
                _menuItemQuantities[menuItem.Id] = menuItem.QuantitySelected + change;
                menuItem.QuantitySelected = _menuItemQuantities[menuItem.Id];
            }

            var filteredItem = FilteredMenuItems.FirstOrDefault(item => item.Id == menuItem.Id && item.Name == menuItem.Name) as OrderMenuItem;
            if (filteredItem != null)
            {
                filteredItem.QuantitySelected = menuItem.QuantitySelected;
            }
            OnPropertyChanged(nameof(FilteredMenuItems));
        }

        private void UpdatePrices(decimal priceChange, decimal discountChange)
        {
            TotalPrice += priceChange;
            Discount += discountChange;
            GrandPrice = TotalPrice - Discount;
        }

        public void ClearOrder()
        {
            OrderItems.Clear();
            TotalPrice = 0;
            GrandPrice = 0;
            foreach (var menuItem in MenuItems.OfType<OrderMenuItem>())
            {
                menuItem.QuantitySelected = 0;
            }
            _menuItemQuantities.Clear();
            ApplyFilter(_currentFilter);
            OnPropertyChanged(nameof(IsOrderItemsNotEmpty));
        }

        public async Task PayOrder(string phoneNumber, string note)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            int EmployeeId = Int32.Parse(localSettings.Values["id"].ToString());
            int orderId = await _orderDao.GetLastOrderId() + 1;
            var order = new Order
            {
                Id = orderId,
                TotalPrice = TotalPrice,
                GrandTotal = GrandPrice,
                Employee = _dao.GetEmployeeById(EmployeeId),
                PhoneNumber = phoneNumber,
                CreatedAt = DateTime.Now,
                Note = note,
                OrderItems = OrderItems.ToList()
            };
            await _orderDao.AddOrder(order);
            await AddOrderDetails(order);
            _postgresDao.CustomerAddPoint(phoneNumber, Convert.ToSingle(GrandPrice) * 0.1f);
            ClearOrder();
        }

        private async Task AddOrderDetails(Order order)
        {
            foreach (var item in OrderItems)
            {
                var orderDetail = new OrderDetail
                {
                    Id = await _orderDetailDao.GetLastOrderDetailId() + 1,
                    Order = order,
                    MenuItem = item.MenuItem,
                    Quantity = item.Quantity,
                    Price = item.Price,
                };
                item.MenuItem.Quantity -= item.Quantity;
                await _orderDetailDao.AddOrderDetail(orderDetail);
            }
        }

        public void SearchMenuItems(string query)
        {
            var filteredItems = string.IsNullOrEmpty(query) ? MenuItems.ToList() : MenuItems.Where(item => item.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            UpdateFilteredMenuItems(filteredItems);
        }
    }
}
