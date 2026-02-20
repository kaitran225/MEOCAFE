using System.ComponentModel;

namespace POS.Models
{
    public class OrderMenuItem : MenuItem, INotifyPropertyChanged
    {
        private int _quantitySelected;
        public int QuantitySelected
        {
            get => _quantitySelected;
            set
            {
                _quantitySelected = value;
                OnPropertyChanged(nameof(QuantitySelected));
            }
        }

        public Microsoft.UI.Xaml.Media.Imaging.BitmapImage? ItemImage { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public decimal? DiscountedPrice
        {
            get
            {
                if (Discount?.IsActive == true)
                {
                    return SellPrice * (1 - Discount.Percentage / 100);
                }
                return null;
            }
            set { }
        }

        public bool? IsCombo { get; set; }
    }

}
