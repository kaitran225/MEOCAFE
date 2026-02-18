using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using POS.Models;
using POS.ViewModels;
using POS.ViewModels.EmployeeViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using Application = Microsoft.UI.Xaml.Application;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace POS.Views.EmployeeViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class OrderHistoryPage : Page, INotifyPropertyChanged
    {
        public OrderHistoryViewModel ViewModel = App.GetService<OrderHistoryViewModel>();
        private const int ItemsPerPage = 10;
        private int _currentPage = 1;
        public bool HasPreviousPage => _currentPage > 1;
        public bool HasNextPage => _currentPage < TotalPages;
        public string PageInfo => $"Trang {_currentPage} / {TotalPages}";
        private int TotalPages => (int)Math.Ceiling((double)ViewModel.AllOrders.Count / ItemsPerPage);

        public event PropertyChangedEventHandler PropertyChanged;

        public OrderHistoryPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void UpdatePagedItems()
        {
            // Clear the current items and add the new paged items
            ViewModel.FilteredOrders.Clear();
            var start = (_currentPage - 1) * ItemsPerPage;
            var pagedData = ViewModel.AllOrders.Skip(start).Take(ItemsPerPage);

            foreach (var item in pagedData)
            {
                ViewModel.FilteredOrders.Add(item);
            }

            // Update the state of the navigation buttons
            OnPropertyChanged(nameof(HasPreviousPage));
            OnPropertyChanged(nameof(HasNextPage));
            OnPropertyChanged(nameof(PageInfo));
        }

        private void PrevOrderPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdatePagedItems();
            }
        }

        private void NextOrderPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < TotalPages)
            {
                _currentPage++;
                UpdatePagedItems();
            }
        }

        private void addOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            // Logic to add a new order
        }

        private async void DeleteOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Delete Order",
                Content = "Are you sure you want to delete selected orders? This action cannot be undone.",
                PrimaryButtonText = "Delete",
                PrimaryButtonStyle = (Style)Application.Current.Resources["RedButtonStyle"],
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (ViewModel.SelectedOrders.Count != 0)
                {
                    await ViewModel.DeleteOrders(ViewModel.SelectedOrders);
                    await Task.Delay(100);
                    SetAllCheckBoxesState(false);
                    ViewModel.SelectedOrders.Clear();
                    OnPropertyChanged(nameof(HasNextPage));
                    ShowSuccessMessage($"Orders deleted successfully.");
                }
                else
                {
                    ShowErrorMessage($"No orders selected.");
                }
            }
        }

        private void OrderHeaderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetAllCheckBoxesState(true);
        }

        private void OrderHeaderCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetAllCheckBoxesState(false);
        }

        private void OrderListItemCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as CheckBox;

            // Retrieve the bound Order model from the DataContext
            if (button?.DataContext is Order order)
            {
                ViewModel.SelectedOrders.Add(order);
            }
        }

        private void OrderListItemCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var button = sender as CheckBox;

            // Retrieve the bound Order model from the DataContext
            if (button?.DataContext is Order order)
            {
                ViewModel.SelectedOrders.Remove(order);
            }
        }

        private void SetAllCheckBoxesState(bool isChecked)
        {
            foreach (var item in ListOrder.Items)
            {
                if (ListOrder.ContainerFromItem(item) is ListViewItem listViewItem)
                {
                    if (listViewItem.ContentTemplateRoot is Grid grid)
                    {
                        var checkBox = grid.Children.OfType<CheckBox>().FirstOrDefault();
                        if (checkBox != null)
                        {
                            checkBox.IsChecked = isChecked;
                        }
                    }
                }
            }
        }

        private async void ShowSuccessMessage(string message)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Success",
                Content = message,
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }

        private async void ShowErrorMessage(string message)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Error",
                Content = message,
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            // Retrieve the bound Order model from the DataContext
            if (button?.DataContext is Order order)
            {
                ViewModel.SelectedOrder = order;
                OrderDetailsTextBlock.Text = $"Order ID: {order.Id}\nTotal Price: {order.TotalPrice}\nNote: {order.Note}\nPhone Number: {order.PhoneNumber}\nCreated At: {order.CreatedAt}\nEmployee: {order.Employee.Fullname}";
                OrderDetailsPanel.Visibility = Visibility.Visible;
                OrderSplitView.IsPaneOpen = true;
            }
        }

        private void CloseOrderDetailsPanelButton_Click(object sender, RoutedEventArgs e)
        {
            OrderDetailsPanel.Visibility = Visibility.Collapsed;
            OrderSplitView.IsPaneOpen = false;
        }

        private void DatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            // Optional: Handle date changed event if needed
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var startDate = StartDatePicker.Date;
            var endDate = EndDatePicker.Date;

            if (startDate.HasValue && endDate.HasValue)
            {
                ViewModel.FilterOrdersByDateRange(startDate.Value.DateTime, endDate.Value.DateTime);
            }
        }
    }
}
