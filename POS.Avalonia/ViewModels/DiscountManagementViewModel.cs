using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class DiscountManagementViewModel : ViewModelBase
{
    private readonly IDiscountRepository _discountRepo;

    [ObservableProperty] private ObservableCollection<Discount> _discounts = new();
    [ObservableProperty] private Discount? _selectedDiscount;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isEditOpen;
    [ObservableProperty] private string _formName = "";
    [ObservableProperty] private string _formDescription = "";
    [ObservableProperty] private string _formPercentage = "0";
    [ObservableProperty] private string _formStartDate = "";
    [ObservableProperty] private string _formEndDate = "";
    [ObservableProperty] private bool _formIsDisabled;
    [ObservableProperty] private bool _isDeleteConfirmOpen;
    [ObservableProperty] private Discount? _deleteTarget;

    public string Title => "Discount management";

    public DiscountManagementViewModel(IDiscountRepository discountRepo)
    {
        _discountRepo = discountRepo;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _discountRepo.GetAllAsync(default).ConfigureAwait(true);
            Discounts = new ObservableCollection<Discount>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void OpenAdd()
    {
        SelectedDiscount = null;
        FormName = "";
        FormDescription = "";
        FormPercentage = "0";
        FormStartDate = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        FormEndDate = DateTime.Today.AddMonths(1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        FormIsDisabled = false;
        IsEditOpen = true;
    }

    [RelayCommand]
    private void OpenEdit(Discount? item)
    {
        if (item == null) return;
        SelectedDiscount = item;
        FormName = item.Name ?? "";
        FormDescription = item.Description ?? "";
        FormPercentage = item.Percentage.ToString(CultureInfo.InvariantCulture);
        FormStartDate = item.StartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        FormEndDate = item.EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        FormIsDisabled = item.IsDisabled;
        IsEditOpen = true;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FormName)) return;
        if (!decimal.TryParse(FormPercentage, NumberStyles.Any, CultureInfo.InvariantCulture, out var pct)) pct = 0;
        var start = DateTime.TryParse(FormStartDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var sd) ? sd : DateTime.Today;
        var end = DateTime.TryParse(FormEndDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var ed) ? ed : DateTime.Today.AddMonths(1);

        if (SelectedDiscount != null)
        {
            SelectedDiscount.Name = FormName.Trim();
            SelectedDiscount.Description = string.IsNullOrWhiteSpace(FormDescription) ? null : FormDescription.Trim();
            SelectedDiscount.Percentage = pct;
            SelectedDiscount.StartDate = start;
            SelectedDiscount.EndDate = end;
            SelectedDiscount.IsDisabled = FormIsDisabled;
            await _discountRepo.UpdateAsync(SelectedDiscount, default).ConfigureAwait(true);
        }
        else
        {
            var d = new Discount
            {
                Name = FormName.Trim(),
                Description = string.IsNullOrWhiteSpace(FormDescription) ? null : FormDescription.Trim(),
                Percentage = pct,
                StartDate = start,
                EndDate = end,
                IsDisabled = FormIsDisabled
            };
            await _discountRepo.AddAsync(d, default).ConfigureAwait(true);
            Discounts.Add(d);
        }
        IsEditOpen = false;
        _ = LoadAsync();
    }

    [RelayCommand]
    private void CloseEdit() => IsEditOpen = false;

    [RelayCommand]
    private void RequestDelete(Discount? item)
    {
        if (item == null) return;
        DeleteTarget = item;
        IsDeleteConfirmOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmDeleteAsync()
    {
        if (DeleteTarget == null) return;
        await _discountRepo.DeleteAsync(DeleteTarget.Id, default).ConfigureAwait(true);
        var toRemove = Discounts.FirstOrDefault(d => d.Id == DeleteTarget.Id);
        if (toRemove != null) Discounts.Remove(toRemove);
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
