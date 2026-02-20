using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using POS.ViewModels.EmployeeViewModels;
using Windows.Services.Maps;
using Windows.UI.Notifications;


namespace POS.Views.EmployeeViews
{


    public partial class ShiftRegister : Page
    {
        private Button[,] shiftButtons;
        public ShiftRegisterViewModel shiftRegisterViewModel { get; set; }
        private readonly string[] weekDays = { "", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN" };
        private readonly string[] timeSlots = { "", "7h - 12h", "12h - 18h", "18h - 22h", "22h - 7h" };

        public ShiftRegister()
        {
            shiftRegisterViewModel = new ShiftRegisterViewModel();
            this.InitializeComponent();
            InitializeShiftGrid();
        }

        private void InitializeShiftGrid()
        {



            shiftButtons = new Button[5, 8];

            // Add headers and buttons
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (row == 0)
                    {
                        // Add weekday headers
                        var headerText = new TextBlock
                        {
                            Text = weekDays[col],
                            Style = Resources["HeaderTextStyle"] as Style
                        };
                        shiftGridPanel.Children.Add(headerText);
                        Grid.SetRow(headerText, row);
                        Grid.SetColumn(headerText, col);
                    }
                    else if (col == 0)
                    {
                        // Add time slot headers
                        var headerText = new TextBlock
                        {
                            Text = timeSlots[row],
                            Style = Resources["HeaderTextStyle"] as Style
                        };
                        shiftGridPanel.Children.Add(headerText);
                        Grid.SetRow(headerText, row);
                        Grid.SetColumn(headerText, col);
                    }
                    else if (row != 0 && col != 0)
                    {
                        // Add shift buttons
                        var button = new Button
                        {
                            Content = "Shift",
                            Foreground = Resources["TextUnregisteredBrush"] as SolidColorBrush,
                            Style = Resources["ShiftButtonStyle"] as Style,
                            Background = Resources["UnregisteredBrush"] as SolidColorBrush,
                            Height = 50,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            Margin = new Thickness(2),
                        };

                        button.Click += ShiftButton_Click;
                        shiftButtons[row, col] = button;
                        shiftGridPanel.Children.Add(button);
                        Grid.SetRow(button, row);
                        Grid.SetColumn(button, col);
                    }
                }
            }
            foreach (var x in shiftRegisterViewModel.FullRegisteredList)
            {
                shiftButtons[x / 8, x % 8].Background = Resources["FullBrush"] as SolidColorBrush;

            }
            foreach (var x in shiftRegisterViewModel.RegisteredDayList)
            {
                shiftButtons[x / 8, x % 8].Background = Resources["RegisteredBrush"] as SolidColorBrush;

            }


        }

        private async void ShiftButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                if (button.Background == Resources["UnregisteredBrush"] as Brush)
                {
                    button.Background = Resources["RegisteredBrush"] as Brush;
                    button.Foreground = Resources["TextregisteredBrush"] as Brush;
                }

                else if (button.Background == Resources["RegisteredBrush"] as Brush)
                {
                    button.Background = Resources["UnregisteredBrush"] as Brush;
                    button.Foreground = Resources["TextUnregisteredBrush"] as Brush;
                }
                else if (button.Background == Resources["FullBrush"] as Brush)
                {
                    NotiMessageTextBlock.Text = "Ca đã đủ số lượng nhân viên, hãy đăng ký ca khác ";
                    await NotifyDialog.ShowAsync();
                }


            }
        }
        private async void CheckinButton_Click(object sender, RoutedEventArgs e)
        {

            shiftRegisterViewModel.checkin();
            NotiMessageTextBlock.Text = "Đã check in !! ";
            await NotifyDialog.ShowAsync();
            return;

        }
        private async void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            shiftRegisterViewModel.checkout();
            NotiMessageTextBlock.Text = "Đã check out !! ";
            await NotifyDialog.ShowAsync();
            return;
        }
        private async void ShiftDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            DateTimeOffset selectedDateOffset = e.NewDate; // Sử dụng trực tiếp NewDate
            DateTime selectedDate = selectedDateOffset.DateTime; // Chuyển đổi sang DateTime
            DateTime today = DateTime.Now.Date;

            if (selectedDate > today)
            {
                ErrorMessageTextBlock.Text = "Ngày đã chọn không hợp lệ. Vui lòng chọn một ngày trong quá khứ.";
                await ErrorDialog.ShowAsync(); // Hiển thị ContentDialog
                return;
            }

            if (selectedDate < today)
            {
                DateTime startOfWeek = selectedDate.AddDays(-(int)selectedDate.DayOfWeek + (int)DayOfWeek.Monday);
                shiftRegisterViewModel.RegisteredDayList.Clear();
                shiftRegisterViewModel.FullRegisteredList.Clear();
                shiftRegisterViewModel.getRegisterShift(startOfWeek);
                shiftRegisterViewModel.getFullRegisteredShift(startOfWeek);
                confirmButton.IsEnabled = startOfWeek != shiftRegisterViewModel.startOfWeek ? false : true;
                InitializeShiftGrid();
            }
        }
        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            List<int> newList = new List<int>();

            for (int row = 1; row < 5; row++)  // Bắt đầu từ 1 để bỏ qua header
            {
                for (int col = 1; col < 8; col++)  // Bắt đầu từ 1 để bỏ qua header
                {
                    var button = shiftButtons[row, col];
                    if (button != null && button.Background == Resources["RegisteredBrush"] as Brush)
                    {
                        shiftRegisterViewModel.RegisteredDayList.Add(row * 8 + col);
                        newList.Add(row * 8 + col);
                    }
                }
            }
            List<int> removeList = shiftRegisterViewModel.RegisteredDayList.Except(newList).ToList();

            shiftRegisterViewModel.RegisterShift();
            foreach (var x in removeList)
            {
                shiftRegisterViewModel.DeleteShift(x);
            }

            NotiMessageTextBlock.Text = "Đã đăng ký thành công   ";
            await NotifyDialog.ShowAsync();
            return;
        }
    }
}