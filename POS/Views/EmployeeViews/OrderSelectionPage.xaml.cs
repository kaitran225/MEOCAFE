using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using POS.Models;
using POS.ViewModels.EmployeeViewModels;
using System.Diagnostics;
using Windows.ApplicationModel.Contacts;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.UI.Xaml.Media.Imaging;
using QRCoder;
using Net.payOS;
using Net.payOS.Types;
using Windows.UI.Notifications;
using POS.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using POS.Services.Dao;

namespace POS.Views.EmployeeViews
{
    public sealed partial class OrderSelectionPage : Page
    {
        public OrderSelectionViewModel ViewModel { get; set; }

        public OrderSelectionPage()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<OrderSelectionViewModel>();
            DataContext = ViewModel;
        }

        private void AllMenuItems_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter("All");
        }

        private void CommboButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter("Combo");
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                ApplyFilter(button.Content.ToString());
            }
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                ModifyOrderItemQuantity(button.DataContext as OrderMenuItem, -1);
            }
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                ModifyOrderItemQuantity(button.DataContext as OrderMenuItem, 1);
            }
        }

        private void Border_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (sender is Border border)
            {
                ShowOrderItemFlyout(border, border.DataContext as OrderItem);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.SearchMenuItems(SearchBox.Text);
        }

        private void ApplyFilter(string filterOption)
        {
            ViewModel.CurrentFilter = filterOption;
            ViewModel.ApplyFilter(filterOption);
        }

        private void ModifyOrderItemQuantity(OrderMenuItem orderItem, int quantityChange)
        {
            if (quantityChange > 0)
            {
                ViewModel.AddItemToOrder(orderItem);
            }
            else
            {
                ViewModel.RemoveItemFromOrder(orderItem);
            }
        }

        private void ShowOrderItemFlyout(Border border, OrderItem orderItem)
        {
            var flyout = new Flyout();
            var contentPanel = new StackPanel { Orientation = Orientation.Horizontal };

            var decreaseButton = new Button
            {
                Content = "−",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Microsoft.UI.Xaml.Thickness(0, 0, 4, 0)
            };
            decreaseButton.Click += (s, args) => ViewModel.RemoveItemFromOrder(orderItem);

            var increaseButton = new Button
            {
                Content = "+",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Microsoft.UI.Xaml.Thickness(4, 0, 0, 0)
            };
            increaseButton.Click += (s, args) => ViewModel.AddItemToOrder(orderItem);

            contentPanel.Children.Add(decreaseButton);
            contentPanel.Children.Add(increaseButton);

            flyout.Content = contentPanel;
            flyout.ShowAt(border);
        }

        private async void pay_Click(object sender, RoutedEventArgs e)
        {
            await PayOrderAsync();

        }

        private async void qr_payment_Click(object sender, RoutedEventArgs e)
        {
            await ProcessQrPaymentAsync();
        }

        private async Task PayOrderAsync()
        {
            string phoneNumber = Phone_Number.Text;
            string note = Note.Text;

            await ViewModel.PayOrder(phoneNumber, note);

            Phone_Number.Text = "";
            Note.Text = "";

            await ShowNotification("Thanh toán thành công!\nHệ thống đã ghi nhận thanh toán!", "SystemFillColorSuccessBrush");
        }

        private async Task ProcessQrPaymentAsync()
        {
            var clientId = Config.GetSetting("CLIENT_ID") ?? "CLIENT_ID";
            var apiKey = Config.GetSetting("API_KEY") ?? "API_KEY";
            var checksumKey = Config.GetSetting("CHECKSUM_KEY") ?? "CHECKSUM_KEY";

            var payOSClient = new Net.payOS.PayOS(clientId, apiKey, checksumKey);

            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
            int amount = (int)(ViewModel.GrandPrice);
            var domain = "https://www.example.com";

            List<ItemData> items = ViewModel.OrderItems.Select(orderItem => new ItemData(
                name: orderItem.MenuItem.Name,
                quantity: orderItem.Quantity,
                price: (int)orderItem.Price
            )).ToList();

            var paymentLinkRequest = new PaymentData(
                orderCode,
                amount,
                description: "Cafe POS Project",
                items: items,
                returnUrl: domain,
                cancelUrl: domain
            );

            var response = await payOSClient.createPaymentLink(paymentLinkRequest);
            BitmapImage qrCode = GenerateQRCode(response.qrCode);

            ContentDialog qrDialog = new()
            {
                Title = "Quét mã QR để thanh toán",
                PrimaryButtonText = "Xác nhận",
                PrimaryButtonStyle = (Style)Microsoft.UI.Xaml.Application.Current.Resources["AccentButtonStyle"],
                CloseButtonText = "Hủy",
                Content = new StackPanel
                {
                    Spacing = 10,
                    Children =
                        {
                            new Microsoft.UI.Xaml.Controls.Image
                            {
                                Source = qrCode,
                                Width = 200,
                                Height = 200,
                                HorizontalAlignment = HorizontalAlignment.Center
                            }
                        }
                },
            };

            CancellationTokenSource cts = new();
            _ = CheckPaymentStatusAsync(orderCode, qrDialog, cts.Token);

            qrDialog.XamlRoot = this.Content.XamlRoot;
            ContentDialogResult result = await qrDialog.ShowAsync();
            cts.Cancel();

            if (result == ContentDialogResult.Primary)
            {
                var payStatus = await payOSClient.getPaymentLinkInformation(orderCode);
                if (payStatus.status == "PAID")
                {
                    await PayOrderAsync();
                }
                else
                {
                    await payOSClient.cancelPaymentLink(orderCode);
                    await ShowNotification("Thanh toán thất bại!\nHệ thống chưa ghi nhận thanh toán!", "SystemFillColorCriticalBrush");
                }
            }
            else
            {
                var payStatus = await payOSClient.getPaymentLinkInformation(orderCode);
                if (payStatus.status == "PENDING")
                {
                    await payOSClient.cancelPaymentLink(orderCode);
                    await ShowNotification("Đã hủy thanh toán!", "SystemFillColorCriticalBrush");
                }
            }
        }

        private async Task CheckPaymentStatusAsync(int orderCode, ContentDialog dialog, CancellationToken token)
        {
            var clientId = Config.GetSetting("CLIENT_ID") ?? "CLIENT_ID";
            var apiKey = Config.GetSetting("API_KEY") ?? "API_KEY";
            var checksumKey = Config.GetSetting("CHECKSUM_KEY") ?? "CHECKSUM_KEY";

            var payOSClient = new Net.payOS.PayOS(clientId, apiKey, checksumKey);

            while (!token.IsCancellationRequested)
            {
                var payStatus = await payOSClient.getPaymentLinkInformation(orderCode);

                if (payStatus.status == "PAID")
                {
                    dialog.Hide();
                    await PayOrderAsync();
                    break;
                }

                await Task.Delay(1000, token);
            }
        }

        private BitmapImage GenerateQRCode(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            using (var qrBitmap = qrCode.GetGraphic(20))
            using (var memoryStream = new MemoryStream())
            {
                qrBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Position = 0;

                BitmapImage qrImage = new BitmapImage();
                qrImage.SetSource(memoryStream.AsRandomAccessStream());
                return qrImage;
            }
        }

        private async Task ShowNotification(string message, string brushResourceKey)
        {
            Noti.Foreground = (Microsoft.UI.Xaml.Media.Brush)Microsoft.UI.Xaml.Application.Current.Resources[brushResourceKey];
            Noti.Text = message;
            Noti.Visibility = Visibility.Visible;
            await Task.Delay(5000);
            Noti.Visibility = Visibility.Collapsed;
        }

        private void MenuBorder_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (e.OriginalSource is Button)
            {
                e.Handled = true;
                return;
            }

            if (sender is Border border)
            {
                ShowMenuItemFlyout(border, border.DataContext as OrderMenuItem);
            }
        }

        private async void ShowMenuItemFlyout(Border border, OrderMenuItem menuItem)
        {
            if (menuItem?.IsCombo == true)
            {
                var _dao = new PostgreSqlDao();
                List<Item> comboItems = _dao.GetComboItems(menuItem.Id);

                // Tạo Grid để hiển thị Combo Items
                var comboGrid = new Grid();
                comboGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Column for Item Name
                comboGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Column for Price

                // Đọc thông tin Combo Items và thêm vào Grid
                int rowIndex = 0;
                decimal totalComboPrice = 0;
                foreach (var item in comboItems)
                {
                    comboGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

                    var nameTextBlock = new TextBlock
                    {
                        Text = item.Name,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetRow(nameTextBlock, rowIndex);
                    Grid.SetColumn(nameTextBlock, 0);
                    comboGrid.Children.Add(nameTextBlock);

                    var priceTextBlock = new TextBlock
                    {
                        Text = $"{item.SellPrice:N0} VNĐ",
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Right
                    };
                    Grid.SetRow(priceTextBlock, rowIndex);
                    Grid.SetColumn(priceTextBlock, 1);
                    comboGrid.Children.Add(priceTextBlock);

                    totalComboPrice += item.SellPrice;
                    rowIndex++;
                }

                // Giả sử giá Combo là tổng giá của các món trong Combo và tiết kiệm được là so với giá trị nguyên gốc
                decimal originalPrice = totalComboPrice; // Giả sử giá gốc là 120% của giá Combo
                decimal comboPrice = menuItem.SellPrice;
                decimal discount = originalPrice - comboPrice;

                // Tạo Grid để hiển thị thông tin Tổng cộng, Giá Combo, Tiết kiệm
                var summaryGrid = new Grid();
                summaryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                summaryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Thêm Tổng cộng vào Grid
                summaryGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                var totalTextBlock = new TextBlock
                {
                    Text = "Tổng cộng:",
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(totalTextBlock, 0);
                Grid.SetColumn(totalTextBlock, 0);
                summaryGrid.Children.Add(totalTextBlock);

                var totalPriceTextBlock = new TextBlock
                {
                    Text = $"{totalComboPrice:N0} VNĐ",
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Right
                };
                Grid.SetRow(totalPriceTextBlock, 0);
                Grid.SetColumn(totalPriceTextBlock, 1);
                summaryGrid.Children.Add(totalPriceTextBlock);

                // Thêm Giá Combo vào Grid
                summaryGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                var comboPriceTextBlock = new TextBlock
                {
                    Text = "Giá Combo:",
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(comboPriceTextBlock, 1);
                Grid.SetColumn(comboPriceTextBlock, 0);
                summaryGrid.Children.Add(comboPriceTextBlock);

                var comboPriceValueTextBlock = new TextBlock
                {
                    Text = $"{comboPrice:N0} VNĐ",
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Right
                };
                Grid.SetRow(comboPriceValueTextBlock, 1);
                Grid.SetColumn(comboPriceValueTextBlock, 1);
                summaryGrid.Children.Add(comboPriceValueTextBlock);

                // Thêm Tiết kiệm vào Grid
                summaryGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                var discountTextBlock = new TextBlock
                {
                    Text = "Tiết kiệm được:",
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(discountTextBlock, 2);
                Grid.SetColumn(discountTextBlock, 0);
                summaryGrid.Children.Add(discountTextBlock);

                var discountValueTextBlock = new TextBlock
                {
                    Text = $"{discount:N0} VNĐ",
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Right,
                    Foreground = (Microsoft.UI.Xaml.Media.Brush)Microsoft.UI.Xaml.Application.Current.Resources["SystemFillColorSuccessBrush"]
                };
                Grid.SetRow(discountValueTextBlock, 2);
                Grid.SetColumn(discountValueTextBlock, 1);
                summaryGrid.Children.Add(discountValueTextBlock);

                // Xây dựng ContentDialog
                var contentDialog = new ContentDialog
                {
                    Title = "Chi tiết Combo",
                    PrimaryButtonText = "Đóng",
                    Content = new StackPanel
                    {
                        Spacing = 10,
                        Children =
                {
                    new TextBlock
                    {
                        Text = "Danh sách món trong Combo:",
                        FontWeight = Microsoft.UI.Text.FontWeights.Bold
                    },
                    comboGrid,
                    new TextBlock
                    {
                        Text = "Thông tin Combo:",
                        FontWeight = Microsoft.UI.Text.FontWeights.Bold
                    },
                    summaryGrid
                }
                    }
                };

                contentDialog.XamlRoot = this.Content.XamlRoot;
                await contentDialog.ShowAsync();
            }
        }

        private void Button_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
