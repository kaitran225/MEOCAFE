using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using POS.Models;
using POS.ViewModels;
using POS.ViewModels.Manager;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using Application = Microsoft.UI.Xaml.Application;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace POS.Views.Manager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class DiscountManagementPage : Page
    {
        public DiscountManagementViewModel ViewModel = App.GetService<DiscountManagementViewModel>();
        private const int ItemsPerPage = 10;
        private int _currentPage = 1;
        public bool HasPreviousPage => _currentPage > 1;
        public bool HasNextPage => _currentPage < TotalPages;
        public string PageInfo => $"Trang {_currentPage} / {TotalPages}";
        private int TotalPages => (int)Math.Ceiling((double)ViewModel.Discounts.Count / ItemsPerPage);
        public DiscountManagementPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void UpdatePagedItems()
        {
            //\.Clear();
            //var start = (_currentPage - 1) * ItemsPerPage;
            //var pagedData = Employees.Skip(start).Take(ItemsPerPage);

            //foreach (var item in pagedData)
            //{
            //    PagedItems.Add(item);
            //}

            //// Cập nhật trạng thái nút
            //OnPropertyChanged(nameof(HasPreviousPage));
            //OnPropertyChanged(nameof(HasNextPage));
            //OnPropertyChanged(nameof(PageInfo));
        }

        private void ChangeSplitPanel(StackPanel panel = null)
        {
            AddDiscountPanel.Visibility = Visibility.Collapsed;
            EditDiscountPanel.Visibility = Visibility.Collapsed;

            if (panel == null)
            {
                ViewModel.IsPaneOpen = false;
            }
            else
            {
                panel.Visibility = Visibility.Visible;
                ViewModel.IsPaneOpen = true;
            }
        }

        private void CloseMenuItemPanelButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeSplitPanel();
        }

        private void ClearAllFields()
        {
            // Add Discount Item
            DiscountNameTextBox.Text = "";
            DiscountDescriptionTextBox.Text = "";
            DiscountPercentageNumberBox.Value = 0;

            // Edit Combo Item
            EditDiscountNameTextBox.Text = "";
            EditDiscountDescriptionTextBox.Text = "";
            EditDiscountPercentageNumberBox.Value = 0;
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdatePagedItems();
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < TotalPages)
            {
                _currentPage++;
                UpdatePagedItems();
            }
        }

        private void addEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeSplitPanel(AddDiscountPanel);
        }   

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Delete Discount",
                Content = "Are you sure you want to delete selected discounts? This action cannot be undone.",
                PrimaryButtonText = "Delete",
                PrimaryButtonStyle = (Style)Application.Current.Resources["RedButtonStyle"],
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (ViewModel.SelectedDiscounts.Count != 0)
                {

                    await ViewModel.DeleteDiscounts(ViewModel.SelectedDiscounts);
                    ViewModel.SelectedDiscounts.Clear();
                    ShowSuccessMessage($"Discounts deleted successfully.");
                }
                else
                {
                    ShowErrorMessage($"No discounts selected.");
                }
            }
        }

        private async void ActionDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            // Retrieve the bound Discount model from the DataContext
            if (button?.DataContext is Discount discount)
            {
                ViewModel.SelectedDiscount = discount;

            }

            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Delete Discount",
                Content = "Are you sure you want to delete this discount? This action cannot be undone.",
                PrimaryButtonText = "Delete",
                PrimaryButtonStyle = (Style)Application.Current.Resources["RedButtonStyle"],
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Proceed with deletion
                var selectedItem = ViewModel.SelectedDiscount;
                if (selectedItem != null)
                {
                    await ViewModel.DeleteDiscount(selectedItem);
                    ShowSuccessMessage($"Discount deleted successfully.");
                }
                else
                {
                    ShowErrorMessage($"No discount selected.");
                }

                ViewModel.IsPaneOpen = false;
            }
        }

        private void EditDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeSplitPanel(EditDiscountPanel);
            var button = sender as Button;

            // Retrieve the bound Discount model from the DataContext
            if (button?.DataContext is Discount discount)
            {
                ViewModel.SelectedDiscount = discount;
                EditDiscountNameTextBox.Text = discount.Name;
                EditDiscountDescriptionTextBox.Text = discount.Description;
                EditDiscountPercentageNumberBox.Value =(double) discount.Percentage;
                EditDiscountStartDatePicker.Date = discount.StartDate;
                EditDiscountEndDatePicker.Date = discount.EndDate;
                EditDiscountIsDisabledCheckBox.IsChecked = discount.IsDisabled;
            }
        }

        private void HeaderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetAllCheckBoxesState(true);
        }

        private void HeaderCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetAllCheckBoxesState(false);
        }

        private void ListItemCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as CheckBox;

            // Retrieve the bound Discount model from the DataContext
            if (button?.DataContext is Discount discount)
            {
                ViewModel.SelectedDiscounts.Add(discount);
            }
        }

        private void ListItemCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var button = sender as CheckBox;

            // Retrieve the bound Discount model from the DataContext
            if (button?.DataContext is Discount discount)
            {
                ViewModel.SelectedDiscounts.Remove(discount);
            }
        }

        private void SetAllCheckBoxesState(bool isChecked)
        {
            foreach (var item in ListDiscount.Items)
            {
                if (ListDiscount.ContainerFromItem(item) is ListViewItem listViewItem)
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



        private async Task<Discount> GetNewDiscount()
        {
            var startDate = DiscountStartDatePicker.Date;
            var endDate = DiscountEndDatePicker.Date;
            if (string.IsNullOrWhiteSpace(DiscountNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(DiscountDescriptionTextBox.Text) ||
                !decimal.TryParse(DiscountPercentageNumberBox.Text, out decimal percentage) ||
                percentage < 0 || percentage > 100 ||
                !startDate.HasValue || !endDate.HasValue
               )
            {
                return null;
            }

            if (startDate.Value > endDate.Value)
            {
                var dialog = new ContentDialog
                {
                    XamlRoot = XamlRoot,
                    Title = "Kiểm tra lại ngày!",
                    Content = "Ngày kết thúc phải sau ngày bắt đầu.",
                    CloseButtonText = "OK",
                };
                await dialog.ShowAsync();
                return null; // Return null if the end date is before the start date
            }

            return new Discount
            {
                Name = DiscountNameTextBox.Text,
                Description = DiscountDescriptionTextBox.Text,
                Percentage = percentage,
                StartDate = startDate.Value.DateTime,
                EndDate = endDate.Value.DateTime,
            };
        }

        private async void SaveDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            var newDiscount = await GetNewDiscount();
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                CloseButtonText = "OK",
            };

            if (newDiscount != null)
            {
                try
                {
                    // Asynchronously add the Discount
                    await ViewModel.AddDiscount(newDiscount);
                    ClearAllFields();
                    // Display success message
                    dialog.Title = "Success";
                    dialog.Content = $"New discount has been added successfully.";

                    // Clear input fields and reset the pane
                    ChangeSplitPanel();
                }
                catch (Exception ex)
                {
                    // Handle any errors that occurred during the add operation
                    dialog.Title = "Error";
                    dialog.Content = $"Failed to add new discount: {ex.Message}";
                }
            }
            else
            {
                // Handle invalid input
                dialog.Title = "Error";
                dialog.Content = "Failed to add new discount. Please check your input.";
            }

            // Show the feedback dialog
            await dialog.ShowAsync();
        }

        private async void DeleteDiscountPanelButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Delete Discount",
                Content = "Are you sure you want to delete this discount? This action cannot be undone.",
                PrimaryButtonText = "Delete",
                PrimaryButtonStyle = (Style)Application.Current.Resources["RedButtonStyle"],
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Proceed with deletion
                var selectedItem = ViewModel.SelectedDiscount;
                if (selectedItem != null)
                {
                    await ViewModel.DeleteDiscount(selectedItem);
                    ShowSuccessMessage($"Discount deleted successfully.");
                }
                else
                {
                    ShowErrorMessage($"No discount selected.");
                }

                ViewModel.IsPaneOpen = false;
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

        private async void SaveEditDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedMenuItem = ViewModel.SelectedDiscount;
            var dialog = new ContentDialog { XamlRoot = XamlRoot, CloseButtonText = "OK", };

            if (!string.IsNullOrWhiteSpace(EditDiscountNameTextBox.Text) &&
                !string.IsNullOrWhiteSpace(EditDiscountDescriptionTextBox.Text) &&
                EditDiscountPercentageNumberBox.Value is var percent and > 0 and < 100 &&
                EditDiscountStartDatePicker.Date.HasValue &&
                EditDiscountEndDatePicker.Date.HasValue &&
                EditDiscountStartDatePicker.Date < EditDiscountEndDatePicker.Date)
            {
                await ViewModel.EditDiscount(selectedMenuItem, EditDiscountNameTextBox.Text, EditDiscountDescriptionTextBox.Text,
                    (decimal)percent, EditDiscountIsDisabledCheckBox.IsChecked ?? false, EditDiscountStartDatePicker.Date.Value.DateTime, EditDiscountEndDatePicker.Date.Value.DateTime);
                dialog.Title = "Success";
                dialog.Content = "Menu item has been edited successfully.";
                ViewModel.IsPaneOpen = false;
            }
            else
            {
                dialog.Title = "Error";
                dialog.Content = "Failed to edit Discount. Please check your input.";
            }

            await dialog.ShowAsync();
        }
    }
}
