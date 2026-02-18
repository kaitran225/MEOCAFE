using Windows.Storage;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using POS.Models;
using POS.ViewModels;
using POS.ViewModels.Manager;
using WinRT.Interop;
using POS.Services;
using POS.ViewModels.EmployeeViewModels;

namespace POS.Views.Manager
{
    public sealed partial class MenuManagementPage : Page
    {
        public OrderHistoryViewModel OrderHistoryViewModel = App.GetService<OrderHistoryViewModel>();
        public MenuManagementViewModel ViewModel = App.GetService<MenuManagementViewModel>();
        private readonly MlModelService _mlModelService= new();
        private readonly ProductExportService _productExportService = new();
        private readonly ProductImportService _productImportService = new();
        private readonly PickerService _pickerService = new();

        public MenuManagementPage()
        {
            DataContext = ViewModel;
            InitializeComponent();

            ViewModel.SelectedCategory = ViewModel.Categories.FirstOrDefault();
            ViewModel.ErrorOccurred += OnErrorOccurred;

        }
        private List<string> _errorMessagesBatch = new List<string>();
        private void OnErrorOccurred(object? sender, string errorMessage)
        {
            _errorMessagesBatch.Add(errorMessage); // Collect error messages for the current batch
        }

        // Show the batch error dialog
        private async Task ShowBatchErrorDialogAsync()
        {
            if (_errorMessagesBatch.Any()) // Check if there were any errors
            {
                // Combine all error messages into a single string
                string aggregatedErrorMessages = string.Join(Environment.NewLine, _errorMessagesBatch);

                // Create the dialog with all errors
                ContentDialog errorDialog = new ContentDialog
                {
                    XamlRoot = XamlRoot,
                    Title = "Errors",
                    Content = aggregatedErrorMessages,
                    CloseButtonText = "OK"
                };

                // Show the dialog and await its closure
                await errorDialog.ShowAsync();

                // Clear the error list after showing the dialog
                _errorMessagesBatch.Clear();
            }
            else
            {
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = XamlRoot,
                    Title = "Info",
                    Content = "Add products successfully",
                    CloseButtonText = "OK"
                };

                // Show the dialog and await its closure
                await dialog.ShowAsync();
            }
        }

        private string _filterOption = "";

        private void Filtered_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                _filterOption = radioButton.Content.ToString();
                ViewModel.ApplyFilter(_filterOption);
            }
        }

        private void ChangeSplitPanel(StackPanel panel = null)
        {
            AddCategoryPanel.Visibility = Visibility.Collapsed;
            AddMenuItemPanel.Visibility = Visibility.Collapsed;
            EditMenuItemPanel.Visibility = Visibility.Collapsed;
            AddComboMenuItemPanel.Visibility = Visibility.Collapsed;
            EditComboItemPanel.Visibility = Visibility.Collapsed;
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

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ItemViewModel clickedMenuItem)
            {
                ViewModel.SelectedItem = clickedMenuItem;
                if (clickedMenuItem.Item is MenuItem)
                {
                    ViewModel.SelectedDiscount = clickedMenuItem.Discount;
                    EditSellPriceNumberBox.Value = (double)clickedMenuItem.SellPrice;
                    EditCapitalPriceNumberBox.Value = (double)clickedMenuItem.CapitalPrice;
                    EditQuantityNumberBox.Value = clickedMenuItem.Quantity;
                    if (clickedMenuItem.Discount is { IsActive: true })
                    {
                        EditDiscountTextBox.Text = GetDiscountFromMenuItem(clickedMenuItem.Discount);
                    }
                    else
                    {
                        EditDiscountTextBox.Text = String.Empty;
                    }

                    EditSelectedImage.Source = clickedMenuItem.Image;
                    ChangeSplitPanel(EditMenuItemPanel);
                }
                else
                {
                    ViewModel.EditSelectedComboItems.Clear();
                    EditComboSellPriceNumberBox.Value = (double)clickedMenuItem.SellPrice;
                    EditComboDescriptionTextBox.Text = clickedMenuItem.Description;
                    EditComboQuantityNumberBox.Value = clickedMenuItem.Quantity;
                    EditComboSelectedImage.Source = clickedMenuItem.Image;
                    ChangeSplitPanel(EditComboItemPanel);
                }
            }
        }

        private void AddMenuItemButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeSplitPanel(_filterOption == "Combo" ? AddComboMenuItemPanel : AddMenuItemPanel);

            ViewModel.IsPaneOpen = true;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ChangeSplitPanel(AddCategoryPanel);
        }

        private void CloseMenuItemPanelButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeSplitPanel();
        }

        private ItemViewModel GetNewMenuItem()
        {
            if (AddMenuItemPanel.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                    !decimal.TryParse(SellPriceNumberBox.Text, out decimal sellPrice) ||
                    !decimal.TryParse(CapitalPriceNumberBox.Text, out decimal capitalPrice) ||
                    !int.TryParse(QuantityNumberBox.Text, out int quantity) ||
                    ViewModel.SelectedCategory == null)
                {
                    return null;
                }

                var menuItem = new MenuItem
                {
                    Name = NameTextBox.Text,
                    SellPrice = sellPrice,
                    CapitalPrice = capitalPrice,
                    Quantity = quantity,
                    Category = ViewModel.SelectedCategory,
                    CategoryId = ViewModel.SelectedCategory.Id
                };
                return new ItemViewModel(menuItem);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(ComboNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ComboDescriptionTextBox.Text) ||
                    !decimal.TryParse(ComboSellPriceNumberBox.Text, out decimal sellPrice) ||
                    !int.TryParse(ComboQuantityNumberBox.Text, out int quantity))
                {
                    return null;
                }

                var menuItem = new ComboMenuItem
                {
                    Name = ComboNameTextBox.Text,
                    SellPrice = sellPrice,
                    Description = ComboDescriptionTextBox.Text,
                    Quantity = quantity,
                };
                return new ItemViewModel(menuItem);
            }
        }

        private void EditQuantityNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            // Ensure the value is an integer
            if (sender.Value % 1 != 0)
            {
                sender.Value = Math.Round(sender.Value, MidpointRounding.ToZero);
            }
        }

        private async void SaveMenuItemButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the new menu item from input
            var newItem = GetNewMenuItem();
            var itemType = newItem.Item is MenuItem ? "menu item" : "combo";
            Image? image = newItem.Item is MenuItem ? SelectedImage : ComboSelectedImage;
            // Create a dialog for feedback
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                CloseButtonText = "OK",
            };

            if (newItem != null)
            {
                try
                {
                    // Asynchronously add the menu item
                    await ViewModel.AddItemAsync(newItem, image);
                    ClearAllFields();
                    // Display success message
                    dialog.Title = "Success";
                    dialog.Content = $"New {itemType} has been added successfully.";

                    // Clear input fields and reset the pane
                    image.Source = null;
                    ViewModel.IsPaneOpen = false;
                }
                catch (Exception ex)
                {
                    // Handle any errors that occurred during the add operation
                    dialog.Title = "Error";
                    dialog.Content = $"Failed to add new {itemType}: {ex.Message}";
                }
            }
            else
            {
                // Handle invalid input
                dialog.Title = "Error";
                dialog.Content = "Failed to add new menu item. Please check your input.";
            }

            // Show the feedback dialog
            await dialog.ShowAsync();
        }

        private void ClearAllFields()
        {
            // Add Category
            CategoryNameTextBox.Text = "";

            // Add Menu Item
            NameTextBox.Text = "";
            ViewModel.SelectedCategory = ViewModel.Categories.FirstOrDefault();
            SellPriceNumberBox.Value = 0;
            CapitalPriceNumberBox.Value = 0;
            QuantityNumberBox.Value = 0;
            SelectedImage.Source = null;

            // Add Combo Item
            ComboNameTextBox.Text = "";
            ViewModel.SelectedComboItems.Clear();
            ComboDescriptionTextBox.Text = "";
            ComboSellPriceNumberBox.Value = 0;
            ComboQuantityNumberBox.Value = 0;
            ComboSelectedImage = null;

            // Edit Combo Item
            ViewModel.Edited = false;
            ViewModel.EditSelectedComboItems.Clear();
        }


        private async void PickAPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var file = await OpenImagePickerAsync();
            if (file != null)
            {
                await SetImageFromFile(file, SelectedImage);
            }
        }

        private async void EditPickAPhotoButton_OnClick(object sender, RoutedEventArgs e)
        {
            var file = await OpenImagePickerAsync();
            if (file != null)
            {
                await SetImageFromFile(file, EditSelectedImage);
            }
        }

        private async void AddPickAPhotoButton_OnClick(object sender, RoutedEventArgs e)
        {
            var file = await OpenImagePickerAsync();
            if (file != null)
            {
                await SetImageFromFile(file, ComboSelectedImage);
            }
        }

        private async void EditComboPickAPhotoButton_OnClick(object sender, RoutedEventArgs e)
        {
            var file = await OpenImagePickerAsync();
            if (file != null)
            {
                await SetImageFromFile(file, EditComboSelectedImage);
            }
        }

        private static async Task<StorageFile> OpenImagePickerAsync()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var window = App.MainWindow;
            var hWnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(picker, hWnd);

            return await picker.PickSingleFileAsync();
        }

        private static async Task SetImageFromFile(StorageFile file, Image imageControl)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(stream);
                var bitmap1 = imageControl.Source;
                imageControl.Source = bitmap;
            }
        }

        private async void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            // Get the Category object from the Tag of the clicked MenuFlyoutItem

            if ((sender as MenuFlyoutItem)?.DataContext is not Category menuItem) return;
            // Show confirmation dialog
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Confirm Delete",
                Content = $"Are you sure you want to delete the category '{menuItem.Name}'?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Call your delete logic here
                await ViewModel.DeleteCategoryAsync(menuItem.Id);
            }
        }



        private async void DeleteMenuItemPanelButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Delete Menu Item",
                Content = "Are you sure you want to delete this menu item? This action cannot be undone.",
                PrimaryButtonText = "Delete",
                PrimaryButtonStyle = (Style)Application.Current.Resources["RedButtonStyle"],
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Proceed with deletion
                var selectedItem = ViewModel.SelectedItem;
                var itemType = selectedItem.Item is MenuItem ? "Menu" : "Combo";
                if (selectedItem != null)
                {
                    await ViewModel.DeleteItemAsync(selectedItem);
                    ShowSuccessMessage($"{itemType} item deleted successfully.");
                }
                else
                {
                    ShowErrorMessage($"No {itemType.ToLower()} item selected.");
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



        private async void SaveCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog { XamlRoot = XamlRoot, CloseButtonText = "OK", };

            if (!string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                try
                {
                    await ViewModel.AddCategoryAsync(CategoryNameTextBox.Text);
                    dialog.Title = "Success";
                    dialog.Content = "New category added successfully.";
                    ViewModel.IsPaneOpen = false;
                }
                catch (Exception ex)
                {
                    // Handle any errors that occurred during the add operation
                    dialog.Title = "Error";
                    dialog.Content = $"Failed to add new category: {ex.Message}";
                }
            }
            else
            {
                dialog.Title = "Error";
                dialog.Content = "Category name cannot be empty.";
            }

            await dialog.ShowAsync();
        }

        private async void EditMenuItemButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedMenuItem = ViewModel.SelectedItem;
            var dialog = new ContentDialog { XamlRoot = XamlRoot, CloseButtonText = "OK", };

            if (!string.IsNullOrWhiteSpace(EditNameTextBox.Text) &&
                EditSellPriceNumberBox.Value is var editSellPrice &&
                EditCapitalPriceNumberBox.Value is var editCapitalPrice &&
                EditQuantityNumberBox.Value is var editQuantity)
            {
                await ViewModel.EditMenuItemAsync(selectedMenuItem, EditNameTextBox.Text, (decimal)editSellPrice, (decimal)editCapitalPrice, (int)editQuantity, EditSelectedImage);
                dialog.Title = "Success";
                dialog.Content = "Menu item has been edited successfully.";
                ViewModel.IsPaneOpen = false;
            }
            else
            {
                dialog.Title = "Error";
                dialog.Content = "Failed to edit menu item. Please check your input.";
            }

            await dialog.ShowAsync();
        }

        private async void EditComboItemButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedMenuItem = ViewModel.SelectedItem;
            var dialog = new ContentDialog { XamlRoot = XamlRoot, CloseButtonText = "OK", };

            if (!string.IsNullOrWhiteSpace(EditComboNameTextBox.Text) &&
                EditComboSellPriceNumberBox.Value is var editSellPrice &&
                !string.IsNullOrWhiteSpace(EditComboDescriptionTextBox.Text) &&
                EditComboQuantityNumberBox.Value is var editQuantity)
            {
                await ViewModel.EditComboItemAsync(selectedMenuItem, EditComboNameTextBox.Text, (decimal)editSellPrice, EditComboDescriptionTextBox.Text, (int)editQuantity, EditComboSelectedImage);
                dialog.Title = "Success";
                dialog.Content = "Combo item has been edited successfully.";
                ViewModel.IsPaneOpen = false;
            }
            else
            {
                dialog.Title = "Error";
                dialog.Content = "Failed to edit combo item. Please check your input.";
            }

            await dialog.ShowAsync();
        }

        private async void PickDiscountButton_OnClick(object sender, RoutedEventArgs e)
        {
            EditDiscountListView.ItemsSource = ViewModel.Discounts;

            EditDiscountListView.SelectedItem = ViewModel.Discounts
                .FirstOrDefault(discount => discount.Id == ViewModel.SelectedDiscount?.Id);

            // Show the dialog
            EditDiscountDialog.XamlRoot = XamlRoot;
            EditDiscountDialog.CloseButtonStyle = (Style)Application.Current.Resources["RedButtonStyle"];

            EditDiscountDialog.Visibility = Visibility.Visible;
            var result = await EditDiscountDialog.ShowAsync();
            EditDiscountDialog.Visibility = Visibility.Collapsed;

            // If "Confirm" is clicked, handle selected items
            if (result != ContentDialogResult.Primary) return;
            if (EditDiscountListView.SelectedItem is Discount selectedDiscount)
            {
                ViewModel.SelectedDiscount = selectedDiscount;
            }

            // Display the formatted text in the ComboDescriptionTextBox
            EditDiscountTextBox.Text = GetDiscountFromMenuItem(ViewModel.SelectedDiscount);
        }

        private string GetDiscountFromMenuItem(Discount? selectedItem)
        {
            if (selectedItem == null) return "";

            return $"{selectedItem.Name}, {selectedItem.Percentage}%";
        }

        private async void PickMenuItemsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var itemViewModels = ViewModel.ItemViewModels.Where(item => !item.IsPlaceholder && item.CategoryId != 0);
            MenuItemsListView.ItemsSource = itemViewModels;

            foreach (var item in itemViewModels)
            {
                if (ViewModel.SelectedComboItems.Contains(item))
                {
                    MenuItemsListView.SelectedItems.Add(item);
                }
            }

            // Show the dialog
            PickMenuItemsDialog.XamlRoot = XamlRoot;
            PickMenuItemsDialog.Visibility = Visibility.Visible;
            var result = await PickMenuItemsDialog.ShowAsync();
            PickMenuItemsDialog.Visibility = Visibility.Collapsed;

            // If "Confirm" is clicked, handle selected items
            if (result != ContentDialogResult.Primary) return;

            var selectedItems = MenuItemsListView.SelectedItems.Cast<ItemViewModel>().ToList();

            // Update the Combo description with the selected items
            ViewModel.SelectedComboItems.Clear();
            foreach (var item in selectedItems)
            {
                ViewModel.SelectedComboItems.Add(item);
            }

            // Display the formatted text in the ComboDescriptionTextBox
            ComboDescriptionTextBox.Text = GetDescriptionFromMenuItem(selectedItems);


        }

        private async void EditPickMenuItemsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var itemViewModels = ViewModel.ItemViewModels.Where(item => !item.IsPlaceholder && item.CategoryId != 0);
            EditMenuItemsListView.ItemsSource = itemViewModels;

            List<ComboItem> comboItems = ViewModel.LoadComboItemsByComboId(ViewModel.SelectedItem.ItemId);
            foreach (var item in itemViewModels)
            {
                if (ViewModel.EditSelectedComboItems.Count != 0)
                {
                    if (ViewModel.EditSelectedComboItems.FirstOrDefault(comboItem => comboItem.ItemId == item.ItemId) != null)
                        EditMenuItemsListView.SelectedItems.Add(item);
                }
                else
                {
                    if (comboItems.FirstOrDefault(comboItem => comboItem.MenuItemId == item.ItemId) != null)
                    {
                        EditMenuItemsListView.SelectedItems.Add(item);
                    }
                }
            }

            // Show the dialog
            EditPickMenuItemsDialog.XamlRoot = XamlRoot;
            EditPickMenuItemsDialog.Visibility = Visibility.Visible;
            var result = await EditPickMenuItemsDialog.ShowAsync();
            EditPickMenuItemsDialog.Visibility = Visibility.Collapsed;

            // If "Confirm" is clicked, handle selected items
            if (result == ContentDialogResult.Primary)
            {
                ViewModel.Edited = true;
                var selectedItems = EditMenuItemsListView.SelectedItems.Cast<ItemViewModel>().ToList();

                ViewModel.EditSelectedComboItems.Clear();
                foreach (var item in selectedItems)
                {
                    ViewModel.EditSelectedComboItems.Add(item);
                }

                // Display the formatted text in the ComboDescriptionTextBox
                EditComboDescriptionTextBox.Text = GetDescriptionFromMenuItem(selectedItems);
            }
        }

        private string GetDescriptionFromMenuItem(List<ItemViewModel> selectedItems)
        {
            string comboDescription;
            if (selectedItems.Count > 1)
            {
                comboDescription = string.Join(", ", selectedItems.Take(selectedItems.Count - 1).Select(item => item.Name)) +
                                   " and " + selectedItems.Last().Name;
            }
            else if (selectedItems.Count == 1)
            {
                comboDescription = selectedItems.First().Name;
            }
            else
            {
                comboDescription = string.Empty;
            }

            return comboDescription;
        }

        private void ClearDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedDiscount = new Discount
            {
                Name = "No discount"
            };

            EditDiscountListView.SelectedItem = null;

            EditDiscountTextBox.Text = "No discount selected.";
        }

        private async void ImportDataButton_Click(object sender, RoutedEventArgs e)
        {
            var file = await _pickerService.PickFileAsync(); // Method to let user select a file

            if (file != null && file.FileType == ".csv")
            {
                try
                {
                    var itemsFromCsv = await _productImportService.ImportFromCsv(file);
                    // Add the imported items to the ViewModel

                    var tasks = itemsFromCsv.Select(item => ViewModel.AddItemAsync(item)).ToList();
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    // If error occurs, capture it
                    OnErrorOccurred(this, ex.Message);
                }
                await ShowBatchErrorDialogAsync();
            }
            else if (file != null && file.FileType == ".xlsx")
            {
                try
                {
                    var itemsFromExcel = await _productImportService.ImportFromExcel(file);

                    var tasks = itemsFromExcel.Select(item => ViewModel.AddItemAsync(item)).ToList();
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    // If error occurs, capture it
                    OnErrorOccurred(this, ex.Message);
                }
                await ShowBatchErrorDialogAsync();
            }
        }

        private async void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {
            var folder = await _pickerService.OpenFileSavePickerAsync();
            if (folder == null)
                return;

            var extension = Path.GetExtension(folder.Path)?.ToLowerInvariant();

            if (extension == ".csv")
            {
                await _productExportService.ExportToCsv(ViewModel.ItemViewModels, folder);
            }
            else if (extension == ".xlsx")
            {
                await _productExportService.ExportToExcel(ViewModel.ItemViewModels, folder);
            }
        }

        private async void TrainDataButton_Click(object sender, RoutedEventArgs e)
        {

            var saveFile = await _pickerService.OpenFileTrainSavePickerAsync();
            if (saveFile == null)
            {
                Console.WriteLine("Save operation was canceled.");
                return;
            }
            List<Order> list = OrderHistoryViewModel.GetOrders();
            _mlModelService.TrainModel(list, saveFile);

        }
    }
}
