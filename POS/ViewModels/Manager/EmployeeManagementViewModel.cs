using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Models;
using POS.Helpers;
using POS.Services.Dao;
using System.Collections.ObjectModel;
using Windows.Services.Maps;

namespace POS.ViewModels.Manager
{
    public partial class EmployeeManagementViewModel : ObservableObject
    {
        private IDao _dao = Factory.GetIDAO();

        [ObservableProperty]
        private Employee _newEmployee;

        [ObservableProperty]
        private ObservableCollection<Employee> _employees;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _totalPages = 1;

        [ObservableProperty]
        private int _totalItems = 0;

        [ObservableProperty]
        private int _itemsPerPage = 10;

        [ObservableProperty]
        private string _pageInfo;

        [ObservableProperty]
        private Employee _selectedEmployee;

        public EmployeeManagementViewModel()
        {

            NewEmployee = new Employee();
            Employees = new ObservableCollection<Employee>();
            LoadData();
        }

        [RelayCommand]
        private void LoadData()
        {
            var allEmployees = _dao.GetEmployees();
            TotalItems = allEmployees.Count;
            TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

            // Xóa các phần tử cũ và thêm mới
            Employees?.Clear();
            foreach (var employee in allEmployees.Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage))
            {
                Employees.Add(employee);
            }

            PageInfo = $"Page {CurrentPage} / {TotalPages}";
        }


        [RelayCommand]
        private void AddEmployee()
        {
            if (string.IsNullOrWhiteSpace(NewEmployee.Username) ||
                string.IsNullOrWhiteSpace(NewEmployee.Fullname))
            {
                // Consider adding error handling or user notification
                return;
            }

            var employee = new Employee
            {
                Username = NewEmployee.Username,
                Fullname = NewEmployee.Fullname,
                Password = NewEmployee.Password,
                PhoneNumber = NewEmployee.PhoneNumber,
                Gender = NewEmployee.Gender,
                Address = NewEmployee.Address,
                Dob = NewEmployee.Dob,
                Role = NewEmployee.Role
            };

            // Persist to database
            bool success = _dao.AddEmployee(employee);
            if (success)
            {
                //Employees.Add(employee);
                ResetNewEmployee();
                LoadData();
            }
        }



        [RelayCommand]
        private void ResetNewEmployee()
        {
            NewEmployee.Username = string.Empty;
            NewEmployee.Fullname = string.Empty;
            NewEmployee.Password = string.Empty;
            NewEmployee.PhoneNumber = string.Empty;
            NewEmployee.Gender = string.Empty;
            NewEmployee.Address = string.Empty;
            NewEmployee.Dob = string.Empty;
            NewEmployee.Role = string.Empty;

            OnPropertyChanged(nameof(NewEmployee));
        }


        public void UpdateEmployee(int id, Employee employee)
        {
            if (SelectedEmployee == null) return;

            bool success = _dao.UpdateEmployee(id, employee);
            if (success)
            {

                LoadData();
            }
        }

        [RelayCommand]
        private void RemoveEmployee()
        {
            if (SelectedEmployee == null) return;

            bool success = _dao.DeleteEmployee(SelectedEmployee.ID);
            if (success)
            {
                Employees.Remove(SelectedEmployee);
                LoadData();
            }
        }

        [RelayCommand]
        public void PrevPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadData();
                OnPropertyChanged(nameof(CurrentPage));

                OnPropertyChanged(nameof(PageInfo));

            }
        }

        [RelayCommand]
        public void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                LoadData();
                OnPropertyChanged(nameof(CurrentPage));

                OnPropertyChanged(nameof(PageInfo));

            }
        }
    }
}