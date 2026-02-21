using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

// TODO: Phase 18 - hash passwords; currently stored plain in DB.
public partial class EmployeeManagementViewModel : ViewModelBase
{
    private readonly IEmployeeRepository _employeeRepo;

    [ObservableProperty] private ObservableCollection<Employee> _employees = new();
    [ObservableProperty] private Employee? _selectedEmployee;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isEditOpen;
    [ObservableProperty] private string _formFullname = "";
    [ObservableProperty] private string _formUsername = "";
    [ObservableProperty] private string _formPassword = "";
    [ObservableProperty] private string _formPhoneNumber = "";
    [ObservableProperty] private string _formRole = "Employee";
    [ObservableProperty] private string _formGender = "";
    [ObservableProperty] private string _formAddress = "";
    [ObservableProperty] private string _formDob = "";
    [ObservableProperty] private bool _isDeleteConfirmOpen;
    [ObservableProperty] private Employee? _deleteTarget;

    public string Title => "Employee management";
    public string[] Roles { get; } = { "Employee", "Manager" };

    public EmployeeManagementViewModel(IEmployeeRepository employeeRepo)
    {
        _employeeRepo = employeeRepo;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _employeeRepo.GetAllAsync(default).ConfigureAwait(true);
            Employees = new ObservableCollection<Employee>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void OpenAdd()
    {
        SelectedEmployee = null;
        FormFullname = "";
        FormUsername = "";
        FormPassword = "";
        FormPhoneNumber = "";
        FormRole = "Employee";
        FormGender = "";
        FormAddress = "";
        FormDob = "";
        IsEditOpen = true;
    }

    [RelayCommand]
    private void OpenEdit(Employee? item)
    {
        if (item == null) return;
        SelectedEmployee = item;
        FormFullname = item.Fullname ?? "";
        FormUsername = item.Username ?? "";
        FormPassword = "";
        FormPhoneNumber = item.PhoneNumber ?? "";
        FormRole = string.IsNullOrEmpty(item.Role) ? "Employee" : item.Role;
        FormGender = item.Gender ?? "";
        FormAddress = item.Address ?? "";
        FormDob = item.Dob ?? "";
        IsEditOpen = true;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FormFullname) || string.IsNullOrWhiteSpace(FormUsername)) return;
        var password = FormPassword;
        if (SelectedEmployee != null && string.IsNullOrWhiteSpace(password))
            password = SelectedEmployee.Password ?? "";

        if (SelectedEmployee != null)
        {
            SelectedEmployee.Fullname = FormFullname.Trim();
            SelectedEmployee.Username = FormUsername.Trim();
            SelectedEmployee.Password = password;
            SelectedEmployee.PhoneNumber = string.IsNullOrWhiteSpace(FormPhoneNumber) ? null : FormPhoneNumber.Trim();
            SelectedEmployee.Role = FormRole.Trim();
            SelectedEmployee.Gender = string.IsNullOrWhiteSpace(FormGender) ? null : FormGender.Trim();
            SelectedEmployee.Address = string.IsNullOrWhiteSpace(FormAddress) ? null : FormAddress.Trim();
            SelectedEmployee.Dob = string.IsNullOrWhiteSpace(FormDob) ? null : FormDob.Trim();
            await _employeeRepo.UpdateAsync(SelectedEmployee, default).ConfigureAwait(true);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(password)) return;
            var e = new Employee
            {
                Fullname = FormFullname.Trim(),
                Username = FormUsername.Trim(),
                Password = password,
                PhoneNumber = string.IsNullOrWhiteSpace(FormPhoneNumber) ? null : FormPhoneNumber.Trim(),
                Role = FormRole.Trim(),
                Gender = string.IsNullOrWhiteSpace(FormGender) ? null : FormGender.Trim(),
                Address = string.IsNullOrWhiteSpace(FormAddress) ? null : FormAddress.Trim(),
                Dob = string.IsNullOrWhiteSpace(FormDob) ? null : FormDob.Trim()
            };
            await _employeeRepo.AddAsync(e, default).ConfigureAwait(true);
            Employees.Add(e);
        }
        IsEditOpen = false;
        _ = LoadAsync();
    }

    [RelayCommand]
    private void CloseEdit() => IsEditOpen = false;

    [RelayCommand]
    private void RequestDelete(Employee? item)
    {
        if (item == null) return;
        DeleteTarget = item;
        IsDeleteConfirmOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmDeleteAsync()
    {
        if (DeleteTarget == null) return;
        await _employeeRepo.DeleteAsync(DeleteTarget.Id, default).ConfigureAwait(true);
        var toRemove = Employees.FirstOrDefault(e => e.Id == DeleteTarget.Id);
        if (toRemove != null) Employees.Remove(toRemove);
        IsDeleteConfirmOpen = false;
        DeleteTarget = null;
    }

    [RelayCommand]
    private void CancelDelete()
    {
        IsDeleteConfirmOpen = false;
        DeleteTarget = null;
    }
}
