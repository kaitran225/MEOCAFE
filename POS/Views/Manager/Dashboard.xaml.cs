using LiveChartsCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using POS.Services;
using POS.ViewModels.Manager;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace POS.Views.Manager
{

    public sealed partial class Dashboard : Page
    {
        public DashboardViewModel ViewModel = App.GetService<DashboardViewModel>();
        private readonly PickerService _pickerService = App.GetService<PickerService>();
        private readonly MlModelService _mlModelService = App.GetService<MlModelService>();
        public Dashboard()
        {
            this.DataContext = ViewModel;
            this.InitializeComponent();
            MonthYearPicker.MinDate = DateTimeOffset.Now;
        }

        private void Combo2_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            comboBox.SelectedIndex = 0;
        }

        private void ComboBoxChart1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedValue != null)
            {
                ViewModel.SelectedChart1 = comboBox.SelectedValue as String;
                if (ViewModel.SelectedChart1 == "week")
                {
                    myCalendarDatePicker.Visibility = Visibility.Visible;
                }
                else
                {
                    myCalendarDatePicker.Visibility = Visibility.Collapsed;

                }
            }
        }
        private void ComboBox_SelectionChanged_Chart3(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedValue != null)
            {
                ViewModel.SelectedChart3 = comboBox.SelectedValue as String;

            }
        }
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            comboBox.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedValue != null)
            {
                ViewModel.SelectedChart2 = comboBox.SelectedValue as String;
            }
        }

        private void myCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null)
            {
                var selectedDateOffset = args.NewDate.Value;

                // Chuyển đổi sang DateTime
                var selectedDate = selectedDateOffset.DateTime;
                ViewModel.DateTimeForWeek = selectedDate;

            }

        }

        private int GetTopProductsCount()
        {
            if (TopProductsComboBox.SelectedItem is ComboBoxItem selectedItem &&
                int.TryParse(selectedItem.Content.ToString(), out int topCount))
            {
                return topCount;
            }
            return 5;
        }


        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadModel();
        }

        private void MonthYearPicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null)
            {
                var selectedDate = sender.Date.Value.DateTime;
                ViewModel.DateTimeForTrainedData = selectedDate;

                sender.Date = ViewModel.DateTimeForTrainedData;
            }

        }

        private async void ImportModelButton_Click(object sender, RoutedEventArgs e)
        {
            var file = await _pickerService.PickFileTrainedDataAsync();
            if (file != null)
            {
                ViewModel.LoadChart4And5(file);
            }
        }
    }
}
