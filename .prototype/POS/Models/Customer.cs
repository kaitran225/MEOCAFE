using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POS.Models
{
    public class Customer : ObservableObject
    {


        private string _fullname;
        public string Fullname
        {
            get => _fullname;
            set => SetProperty(ref _fullname, value);
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        private string _point;
        public string Point
        {
            get => _point;
            set => SetProperty(ref _point, value);
        }




    }
}