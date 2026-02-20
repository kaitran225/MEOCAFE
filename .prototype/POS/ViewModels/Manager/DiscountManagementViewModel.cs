using CommunityToolkit.Mvvm.ComponentModel;
using POS.Models;
using POS.Services.Dao;
using System.Collections.ObjectModel;

namespace POS.ViewModels.Manager
{
    public partial class DiscountManagementViewModel : ObservableRecipient
    {
        private readonly IDao _dao;
        public ObservableCollection<Discount> Discounts { get; private set; } = new();
        public ObservableCollection<Discount> FilteredDiscounts { get; private set; } = new();
        public Discount SelectedDiscount;
        public List<Discount> SelectedDiscounts = new();
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    FilterDiscounts();
                }
            }
        }

        private bool _isPaneOpen;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => SetProperty(ref _isPaneOpen, value);
        }

        public DiscountManagementViewModel(IDao dao)
        {
            _dao = dao;
            LoadDiscounts(1, 10);
            FilterDiscounts();
            Discounts.CollectionChanged += (sender, args) => FilterDiscounts();
        }

        public void LoadDiscounts(int page, int itemsPerPage)
        {
            Discounts = new ObservableCollection<Discount>(_dao.GetDiscounts().Skip((page - 1) * itemsPerPage).Take(itemsPerPage));
        }

        public async Task AddDiscount(Discount discount)
        {
            await _dao.AddDiscountAsync(discount);
            Discounts.Add(discount);
        }

        public async Task EditDiscount(Discount discount, string newName, string newDescription, decimal newPercentage, bool isDisabled,
            DateTime newStartDate, DateTime newEndDate)
        {
            Discount newDiscount = new()
            {
                Id = discount.Id,
                Name = newName,
                Description = newDescription,
                Percentage = newPercentage,
                IsDisabled = isDisabled,
                StartDate = newStartDate,
                EndDate = newEndDate
            };
            await _dao.EditDiscountAsync(newDiscount);

            discount.Name = newName;
            discount.Description = newDescription;
            discount.Percentage = newPercentage;
            discount.IsDisabled = isDisabled;
            discount.StartDate = newStartDate;
            discount.EndDate = newEndDate;
            FilterDiscounts();
        }

        public async Task DeleteDiscount(Discount discount)
        {
            await _dao.DeleteDiscountAsync(discount.Id);
            Discounts.Remove(discount);
            FilterDiscounts();
        }

        public async Task DeleteDiscounts(List<Discount> discounts)
        {
            foreach (var selectedDiscount in discounts)
            {
                await DeleteDiscount(selectedDiscount);
            }
            FilterDiscounts();
        }

        private void FilterDiscounts()
        {

            // If there is a search term, apply the search filter to the category-filtered items
            var filteredItems = Discounts.ToList();

            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                filteredItems = Discounts
                    .Where(item => item.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Update the filtered items
            UpdateFilteredMenuItems(filteredItems);
        }

        private void UpdateFilteredMenuItems(List<Discount> items)
        {
            FilteredDiscounts.Clear();

            // Add the filtered items to the collection
            foreach (var item in items)
            {
                FilteredDiscounts.Add(item);
            }
        }
    }
}
