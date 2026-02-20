using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POS.Models
{
    public class OrderItem : INotifyPropertyChanged
    {
        private int _quantity;

        public OrderMenuItem MenuItem { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isCombo = false;
        public bool IsCombo
        {
            get => _isCombo;
            set
            {
                if (_isCombo != value)
                {
                    _isCombo = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
