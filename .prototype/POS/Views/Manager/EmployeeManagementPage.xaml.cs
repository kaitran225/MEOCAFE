using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using POS.ViewModels.Manager;
using POS.Models;

namespace POS.Views.Manager
{
    public sealed partial class EmployeeManagementPage : Page
    {
        public EmployeeManagementViewModel ViewModel { get; set; }

        public EmployeeManagementPage()
        {
            this.InitializeComponent();
            ViewModel = new EmployeeManagementViewModel();
            DataContext = ViewModel;
        }
        private void ScrollViewer_ViewChanged_1(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }


        private void SearchEmployee_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Implement search functionality if needed
            }
        }
        private void SearchEmployee_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }
        private void SearchEmployee_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {

                sender.Text = args.ChosenSuggestion.ToString();
            }
            else
            {

                var userInput = args.QueryText;

            }
        }

        private void UpdateDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Close dialog without saving
        }
        private void PreButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PrevPage();
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NextPage();
        }

        private void addEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleAddEmployeeForm();
        }

        private void ToggleAddEmployeeForm()
        {
            if (AddEditEmployeeForm.Visibility == Visibility.Visible)
            {
                AddEditEmployeeForm.Visibility = Visibility.Collapsed;
                Grid.SetColumnSpan(scrollviewListEmployee, 2);
            }
            else
            {
                AddEditEmployeeForm.Visibility = Visibility.Visible;
                Grid.SetColumnSpan(scrollviewListEmployee, 1);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddEmployeeCommand.Execute(null);
            ToggleAddEmployeeForm();
        }

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleAddEmployeeForm();
            ViewModel.ResetNewEmployeeCommand.Execute(null);
        }

        private async void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is POS.Models.Employee employee)
            {
                ViewModel.SelectedEmployee = employee;


                UpdateFullName.Text = employee.Fullname;
                UpdateUsername.Text = employee.Username;
                UpdatePhoneNumber.Text = employee.PhoneNumber;
                UpdateAddress.Text = employee.Address;
                UpdateDob.Text = employee.Dob;
                UpdateGender.SelectedItem = employee.Gender == "Male"
                    ? UpdateGender.Items[0]
                    : UpdateGender.Items[1];
                UpdateRole.Text = employee.Role;

                await UpdateEmployeeDialog.ShowAsync();
            }
        }

        private void UpdateDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (ViewModel.SelectedEmployee != null)
            {

                POS.Models.Employee updateEmployee = new POS.Models.Employee();
                updateEmployee.Fullname = UpdateFullName.Text;
                updateEmployee.Username = UpdateUsername.Text;
                updateEmployee.PhoneNumber = UpdatePhoneNumber.Text;
                updateEmployee.Address = UpdateAddress.Text;
                updateEmployee.Dob = UpdateDob.Text;
                updateEmployee.Gender = (UpdateGender.SelectedItem as ComboBoxItem)?.Content.ToString();
                updateEmployee.Role = UpdateRole.Text;
                ViewModel.UpdateEmployee(int.Parse(ViewModel.SelectedEmployee.ID), updateEmployee);
            }
        }

        private void DeleteBtnEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is POS.Models.Employee employee)
            {
                ViewModel.SelectedEmployee = employee;
                ViewModel.RemoveEmployeeCommand.Execute(null);
            }
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {


        }
        private void printBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private void exportPdfBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var employeesToDelete = new List<POS.Models.Employee>();

            // Duyệt qua tất cả các item trong ListEmployee và tìm các mục có CheckBox được chọn
            foreach (var item in ViewModel.Employees) // Sử dụng ViewModel.Employees thay vì ListEmployee.Items
            {
                if (ListEmployee.ContainerFromItem(item) is ListViewItem listViewItem)
                {
                    if (listViewItem.ContentTemplateRoot is Grid grid)
                    {
                        var checkBox = grid.Children.OfType<CheckBox>().FirstOrDefault();
                        if (checkBox != null && checkBox.IsChecked == true)
                        {
                            employeesToDelete.Add(item);
                        }
                    }
                }
            }

            // Xóa nhân viên đã chọn
            foreach (var employee in employeesToDelete)
            {
                ViewModel.SelectedEmployee = employee;
                ViewModel.RemoveEmployeeCommand.Execute(null);
            }

            // Đặt lại tất cả CheckBox thành Unchecked
            SetAllCheckBoxesState(false);
        }

        private void HeaderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetAllCheckBoxesState(true);
        }

        private void HeaderCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetAllCheckBoxesState(false);
        }

        private void SetAllCheckBoxesState(bool isChecked)
        {
            foreach (var item in ListEmployee.Items)
            {
                if (ListEmployee.ContainerFromItem(item) is ListViewItem listViewItem)
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
    }
}