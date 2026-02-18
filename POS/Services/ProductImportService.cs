using ClosedXML.Excel;
using CsvHelper;
using POS.Models;
using POS.ViewModels;
using System.Globalization;
using Windows.Storage;

namespace POS.Services
{
    public class ProductImportService
    {
        public async Task<List<ItemViewModel>> ImportFromCsv(StorageFile file)
        {
            await using var stream = await file.OpenStreamForReadAsync();
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<MenuItemDtoModel>();
            List<ItemViewModel> importedItems = GetImportableItemViewModels(records).ToList();

            return importedItems;
        }

        public async Task<List<ItemViewModel>> ImportFromExcel(StorageFile file)
        {
            var importedItems = new List<ItemViewModel>();

            await using var stream = await file.OpenStreamForReadAsync();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RowsUsed().Skip(1); // Skip header row

            foreach (var row in rows)
            {
                // Read and validate each cell value
                string name = row.Cell(1).GetValue<string>().Trim();
                bool isSellPriceValid = decimal.TryParse(row.Cell(2).GetValue<string>(), out decimal sellPrice);
                bool isCapitalPriceValid = decimal.TryParse(row.Cell(3).GetValue<string>(), out decimal capitalPrice);
                bool isQuantityValid = int.TryParse(row.Cell(4).GetValue<string>(), out int quantity);
                bool isCategoryIdValid = int.TryParse(row.Cell(5).GetValue<string>(), out int categoryId);

                // Ensure all required fields are valid before adding the item
                if (!isSellPriceValid || !isCapitalPriceValid || !isQuantityValid || !isCategoryIdValid)
                {
                    Console.WriteLine($"Invalid data in row: {row.RowNumber()}");
                    continue; // Skip invalid rows
                }

                // Create the item
                var item = new MenuItem
                {
                    Name = name,
                    SellPrice = sellPrice,
                    CapitalPrice = capitalPrice,
                    Quantity = quantity,
                    CategoryId = categoryId
                };

                // Convert to ViewModel and add to the list
                importedItems.Add(new ItemViewModel(item));
            }
            return importedItems;
        }
        private IEnumerable<ItemViewModel> GetImportableItemViewModels(IEnumerable<MenuItemDtoModel> products)
        {
            return products
                .Select(menuItem => new ItemViewModel(new MenuItem
                {
                    Name = menuItem.Name,
                    CategoryId = menuItem.CategoryId,
                    CapitalPrice = menuItem.CapitalPrice,
                    SellPrice = menuItem.SellPrice,
                    Quantity = menuItem.Quantity
                }));
        }
    }
}
