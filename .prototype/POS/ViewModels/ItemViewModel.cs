using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using POS.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace POS.ViewModels
{
    public class ItemViewModel : ObservableObject
    {
        public Item Item { get; }

        public ItemViewModel(Item item)
        {
            Item = item;

            // Initialize properties
            _name = Item.Name;
            _sellPrice = Item.SellPrice;
            switch (item)
            {
                case MenuItem menu:
                    _capitalPrice = menu.CapitalPrice;
                    CategoryId = menu.CategoryId;
                    IsPlaceholder = menu.IsPlaceholder;
                    _discount = menu.Discount;
                    break;
                case ComboMenuItem comboMenuItem:
                    _description = comboMenuItem.Description;
                    break;
            }

            _quantity = Item.Quantity;

            LoadImageAsync();
        }

        // Properties linked to the Item model
        public int ItemId => Item.Id;
        public int CategoryId;
        private Discount? _discount;
        public Discount? Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private decimal _sellPrice;
        public decimal SellPrice
        {
            get => _sellPrice;
            set => SetProperty(ref _sellPrice, value);
        }

        private decimal _capitalPrice;
        public decimal CapitalPrice
        {
            get => _capitalPrice;
            set => SetProperty(ref _capitalPrice, value);
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get => _image;
            private set => SetProperty(ref _image, value);
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
        }

        public bool IsPlaceholder;

        // Loads an image asynchronously
        public async Task LoadImageAsync()
        {
            if (Item.Image?.Length > 0)
            {
                var decodedImage = DecodeBase64(Encoding.UTF8.GetString(Item.Image)) ?? Item.Image;
                Image = await ConvertByteArrayToImageAsync(decodedImage);
            }
            else
            {
                Image = GetFallbackImage();
            }
        }

        // Saves an image by converting it from byte array
        public async void SaveImage(byte[]? imageBytes)
        {
            if (imageBytes?.Length > 0)
            {
                var decodedImage = DecodeBase64(Encoding.UTF8.GetString(imageBytes)) ?? imageBytes;
                Image = await ConvertByteArrayToImageAsync(decodedImage);
            }
            else
            {
                Image = GetFallbackImage();
            }
        }

        // Decodes Base64 string to byte array
        private byte[]? DecodeBase64(string base64Data)
        {
            try
            {
                return Convert.FromBase64String(base64Data);
            }
            catch
            {
                Debug.WriteLine("Failed to decode Base64 string.");
                return null;
            }
        }

        // Converts a byte array to a BitmapImage
        private async Task<BitmapImage> ConvertByteArrayToImageAsync(byte[] byteArray)
        {
            try
            {
                using var stream = new MemoryStream(byteArray);
                using var randomAccessStream = stream.AsRandomAccessStream();

                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(randomAccessStream);

                return bitmapImage;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to convert byte array to image: {ex.Message}");
                return GetFallbackImage();
            }
        }

        // Provides a fallback image in case of an error
        private BitmapImage GetFallbackImage()
        {
            return new BitmapImage(new Uri("ms-appx:///Assets/placeholder.jpg"));
        }
    }
}
