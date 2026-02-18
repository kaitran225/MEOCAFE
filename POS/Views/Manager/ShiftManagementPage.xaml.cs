using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using POS.ViewModels.EmployeeViewModels;
using POS.ViewModels.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace POS.Views.Manager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShiftManagementPage : Page
    {
        private Button[,] shiftButtons;
        public ShiftManagementViewModel shiftManagementViewModel { get; set; }
        private readonly string[] weekDays = { "", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN" };
        private readonly string[] timeSlots = { "", "7h - 12h", "12h - 18h", "18h - 22h", "22h - 7h" };

        public ShiftManagementPage()
        {
            shiftManagementViewModel = new ShiftManagementViewModel();
            this.InitializeComponent();
            InitializeShiftGrid();
            DataContext = shiftManagementViewModel;
        }
        private void ScrollViewer_ViewChanged_2(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }
        private void InitializeShiftGrid()
        {
            shiftGridPanel.Children.Clear();

            shiftButtons = new Button[5, 8];
            int currentIndex = 0;

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
                        string listemployee = shiftManagementViewModel.FullRegisteredList[currentIndex];
                        int employeeCount = listemployee.Split(',').Length - 1;
                        string output = listemployee.Replace(",", " ");

                        // Add shift buttons
                        var button = new Button
                        {
                            Foreground = (employeeCount == 5)
                                          ? Resources["TextFULL"] as SolidColorBrush
                                          : Resources["TextNONFULL"] as SolidColorBrush,
                            Style = Resources["ShiftButtonStyle"] as Style,
                            Background = (employeeCount == 5)
                                          ? Resources["FULL"] as SolidColorBrush
                                          : Resources["NONFULL"] as SolidColorBrush,
                            Height = 80,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            Margin = new Thickness(2),
                        };
                        var textBlock = new TextBlock
                        {
                            Text = output, // Văn bản hiển thị
                            TextWrapping = TextWrapping.Wrap, // Cho phép xuống dòng
                            TextAlignment = TextAlignment.Center, // Căn giữa văn bản
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        };

                        button.Content = textBlock; // Gán TextBlock làm nội dung của nút
                        //button.Click += ShiftButton_Click;
                        shiftButtons[row, col] = button;
                        shiftGridPanel.Children.Add(button);
                        Grid.SetRow(button, row);
                        Grid.SetColumn(button, col);
                        currentIndex++;
                    }
                }
            }




        }



        private async void ShiftDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            DateTimeOffset selectedDateOffset = e.NewDate; // Sử dụng trực tiếp NewDate
            DateTime selectedDate = selectedDateOffset.DateTime; // Chuyển đổi sang DateTime
            DateTime today = DateTime.Now.Date;

            if (selectedDate > today)
            {
                ErrorMessageTextBlock.Text = "Ngày đã chọn không hợp lệ. Vui lòng chọn một ngày trong quá khứ.";
                await ErrorDialog.ShowAsync();
                return;
            }

            if (selectedDate < today)
            {
                DateTime startOfWeek = selectedDate.AddDays(-(int)selectedDate.DayOfWeek + (int)DayOfWeek.Monday);
                shiftManagementViewModel.getAll(startOfWeek);
                InitializeShiftGrid();
            }
        }



    }
}
