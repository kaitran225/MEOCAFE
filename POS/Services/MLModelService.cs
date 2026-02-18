using Windows.Storage;
using Microsoft.ML;
using POS.Models;
using System.Windows.Controls;
using Microsoft.ML.Data;
using System.Linq;

namespace POS.Services
{
    public class DrinkSalesData
    {
        public float Month { get; set; }
        public float Year { get; set; }
        public float SellPrice { get; set; }
        public float CapitalPrice { get; set; }
        public float QuantitySold { get; set; }
        public string Category { get; set; }
    }

    public class DrinkSalesPrediction
    {
        [ColumnName("Score")]
        public float PredictedQuantitySold { get; set; }
    }

    public class ProductPrediction
    {
        public string ProductName { get; set; }
        public float PredictedSales { get; set; }
    }


    public class MlModelService
    {
        private readonly MLContext _mlContext = new();

        public async void TrainModel(IEnumerable<Order> orders, StorageFile file)
        {
            try
            {
                // Consolidate data into DrinkSalesData
                var trainingData = orders.SelectMany(order => order.OrderItems.Select(item => new DrinkSalesData
                {
                    Month = order.CreatedAt.Month,
                    Year = order.CreatedAt.Year,
                    SellPrice = (float)item.MenuItem.SellPrice,
                    CapitalPrice = (float)item.MenuItem.CapitalPrice,
                    QuantitySold = item.Quantity, // This is the label
                    Category = item.MenuItem.Category.Name
                })).ToList();

                MLContext mlContext = new();
                // Load data into ML.NET
                IDataView dataView = mlContext.Data.LoadFromEnumerable(trainingData);

                // Define the training pipeline
                var pipeline = mlContext.Transforms.Categorical.OneHotEncoding("CategoryEncoded", "Category")
                    .Append(mlContext.Transforms.Concatenate("Features", "Month", "Year", "SellPrice", "CapitalPrice", "CategoryEncoded"))
                    .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "QuantitySold",
                        maximumNumberOfIterations: 100));

                // Train the model
                var model = pipeline.Fit(dataView);

                var inputData = new DrinkSalesData
                {
                    Month = 1,
                    Year = 2025,
                    SellPrice = 40000.00f,
                    CapitalPrice = 5000.00f,
                    Category = orders.First().OrderItems.First().MenuItem.Category.Name
                };

                var predictionEngine = mlContext.Model.CreatePredictionEngine<DrinkSalesData, DrinkSalesPrediction>(model);

                var prediction = predictionEngine.Predict(inputData);

                Console.WriteLine($"Predicted sales: {prediction.PredictedQuantitySold}");

                var assetsFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                var modelFileInAssets = await assetsFolder.CreateFileAsync("SalesModel.zip", CreationCollisionOption.ReplaceExisting);

                await Task.Run(() =>
                {
                    using var stream = file.OpenStreamForWriteAsync().Result;
                    using var assetStream = modelFileInAssets.OpenStreamForWriteAsync().Result;
                    mlContext.Model.Save(model, dataView.Schema, stream);
                    _mlContext.Model.Save(model, dataView.Schema, assetStream);
                });

                Console.WriteLine($"Model trained and saved successfully at {file.Path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during training: {ex.Message}");
            }
        }

        public async Task<List<ProductPrediction>> RecommendPopularDrinksAsync(HashSet<OrderMenuItem> menuItems, DateTime nextMonth, int count, StorageFile modelFile)
        {
            var predictions = new List<ProductPrediction>();
            try
            {
                // Check if the model file exists
                if (modelFile == null)
                {
                    Console.WriteLine("Model file not provided. Please select a valid model file.");
                    return new List<ProductPrediction>();
                }

                // Load the trained model
                ITransformer model;
                using (var stream = await modelFile.OpenStreamForReadAsync())
                {
                    model = _mlContext.Model.Load(stream, out var _);
                }

                var assetsFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                var modelFileInAssets = await assetsFolder.CreateFileAsync("SalesModel.zip", CreationCollisionOption.ReplaceExisting);

                using (var assetStream = await modelFileInAssets.OpenStreamForWriteAsync())
                {
                    _mlContext.Model.Save(model, null, assetStream);
                }


                // Create a prediction engine
                var predictionEngine = _mlContext.Model.CreatePredictionEngine<DrinkSalesData, DrinkSalesPrediction>(model);

                // Generate predictions for each menu item
                foreach (var menuItem in menuItems)
                {
                    var inputData = new DrinkSalesData
                    {
                        Month = nextMonth.Month,
                        Year = nextMonth.Year,
                        SellPrice = (float)menuItem.SellPrice,
                        CapitalPrice = (float)menuItem.CapitalPrice,
                        Category = menuItem.Category?.Name ?? "Unknown"
                    };

                    var predictedSales = predictionEngine.Predict(inputData).PredictedQuantitySold;

                    predictions.Add(new ProductPrediction
                    {
                        ProductName = menuItem.Name,
                        PredictedSales = predictedSales
                    });
                }

                // Sort predictions by sales and select the top items
                return predictions
                    .OrderByDescending(p => p.PredictedSales)
                    .Take(count)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RecommendPopularDrinksAsync: {ex.Message}");
                return new List<ProductPrediction>();
            }
        }
    }
}
