using System.Globalization;
using CsvHelper;
using OfficeOpenXml;
using POS.Models;
using POS.ViewModels;
using Windows.Storage;

namespace POS.Services
{
    public class ProductExportService
    {
        public async Task ExportToCsv(IEnumerable<ItemViewModel> products, StorageFile file)
        {
            await using var writer = new StreamWriter(await file.OpenStreamForWriteAsync());
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(GetExportableMenuItems(products));
        }

        public async Task ExportToExcel(IEnumerable<ItemViewModel> products, StorageFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Menu Items");
            worksheet.Cells["A1"].LoadFromCollection(GetExportableMenuItems(products), true);
            await using var stream = await file.OpenStreamForWriteAsync();
            await package.SaveAsAsync(stream);
        }

        private IEnumerable<MenuItemDtoModel> GetExportableMenuItems(IEnumerable<ItemViewModel> products)
        {
            return products
                .Where(item => item is { Item: MenuItem, IsPlaceholder: false })
                .Select(item => item.Item as MenuItem)
                .Where(menuItem => menuItem != null)
                .Select(menuItem => new MenuItemDtoModel
                {
                    Id = menuItem!.Id,
                    Name = menuItem.Name,
                    Quantity = menuItem.Quantity,
                    CategoryId = menuItem.CategoryId,
                    CapitalPrice = menuItem.CapitalPrice,
                    SellPrice = menuItem.SellPrice,
                });
        }
    }
}
