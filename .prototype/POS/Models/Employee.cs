using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POS.Models
{
    public class Employee : ObservableObject
    {
        private string _id;
        public string ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _fullname;
        public string Fullname
        {
            get => _fullname;
            set => SetProperty(ref _fullname, value);
        }

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        private string _gender;
        public string Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        private string _address;
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private string _dob;
        public string Dob
        {
            get => _dob;
            set => SetProperty(ref _dob, value);
        }

        private string _role;
        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }
    }
}