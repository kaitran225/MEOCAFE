using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Microsoft.UI.Xaml;
using POS.Models;
using POS.Services;
using POS.Services.Dao;
using SkiaSharp;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.Storage;

namespace POS.ViewModels.Manager
{

    public class DashboardViewModel : ObservableObject
    {
        public DashboardViewModel(IDao dao, MlModelService mlModelService)
        {
            Dao = dao;
            MlModelService = mlModelService;
            DateTimeForWeek = DateTime.Now;
            DateTimeForTrainedData = DateTime.Now;
            TotalSales = dao.getTotalSales();
            AverageAvenuePerTransaction = dao.AverageRevenuePerTransaction();
            CustomerReturning = dao.CustomerReturning() * 100;
            TopSellingProducts = dao.getTopSelling();
            KeyProduct = TopSellingProducts.Key;
            RevenueKeyProduct = TopSellingProducts.Value;
            TopProductsCount = 5;
            SetLegendTextPaint();
            TopProductsSeries = new ISeries[] { };
            LoadChart1();
            loadChart2();
            loadChart3();
            LoadModel();
        }

        private int _topProductsCount;
        public int TopProductsCount
        {
            get => _topProductsCount;
            set
            {
                if (_topProductsCount != value)
                {
                    _topProductsCount = value;
                    OnPropertyChanged(nameof(TopProductsCount));
                }
            }
        }

        public MlModelService MlModelService;
        public ISeries[] TopProductsSeries { get; set; }
        public IDao Dao;
        public ISeries[] Chart3 { get; set; }
        public float TotalSales { get; set; }
        public float AverageAvenuePerTransaction { get; set; }
        public float CustomerReturning { get; set; }
        public KeyValuePair<string, float> TopSellingProducts { get; set; }
        private string keyProduct;
        public string KeyProduct
        {
            get; set;
        }
        private float revenueKeyProduct;
        public float RevenueKeyProduct
        {
            get; set;
        }


        private DateTime _dateTimeForWeek;
        public DateTime DateTimeForWeek
        {
            set
            {
                if (_dateTimeForWeek != value)
                {
                    _dateTimeForWeek = value;
                    LoadChart1();
                }
            }
            get => _dateTimeForWeek;
        }




        private string selectedChart1;
        public string SelectedChart1
        {
            get => selectedChart1;
            set
            {
                if (selectedChart1 != value)
                {
                    selectedChart1 = value;
                    OnPropertyChanged(nameof(SelectedChart1)); // Thông báo thay đổi
                    LoadChart1(); // Cập nhật biểu đồ khi giá trị thay đổi
                }
            }
        }

        private string selectedChart2;
        public string SelectedChart2
        {
            get => selectedChart2;
            set
            {
                if (selectedChart2 != value)
                {
                    selectedChart2 = value;
                    OnPropertyChanged(nameof(selectedChart2)); // Thông báo thay đổi
                    loadChart2(); // Cập nhật biểu đồ khi giá trị thay đổi
                }
            }
        }

        private string selectedChart3;
        public string SelectedChart3
        {
            get => selectedChart3;
            set
            {
                if (selectedChart3 != value)
                {
                    selectedChart3 = value;
                    OnPropertyChanged(nameof(selectedChart3)); // Thông báo thay đổi
                    loadChart3(); // Cập nhật biểu đồ khi giá trị thay đổi
                }
            }
        }

        public List<Tuple<string, string>> SelectedTimePeriod { get; set; } = new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("Week", "week"),
            new Tuple<string, string>("Month", "month"),
            new Tuple<string, string>("Year", "year")
        };
        public List<Tuple<string, string>> SelectedTimePeriod2 { get; set; } = new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("Week", "week"),
            new Tuple<string, string>("Month", "month"),
            new Tuple<string, string>("Year", "year")
        };
        public List<Tuple<string, string>> SelectedTimePeriod3 { get; set; } = new List<Tuple<string, string>>()
        {

            new Tuple<string, string>("Month", "month"),
            new Tuple<string, string>("Year", "year")
        };

        public ISeries[] Chart1 { get; set; }
        public LabelVisual TitleChart1 { get; set; } =
            new LabelVisual
            {
                Text = "Statistics for revenue ",
                Paint = new SolidColorPaint(SKColor.Parse("#FF5733")), // Màu cam đậm
                TextSize = 20,
                Padding = new LiveChartsCore.Drawing.Padding(0, 0, 0, 20), // Tăng khoảng cách dưới
                Y = 50,
            };
        public List<Axis> XAxesChart1 { get; set; }
        public List<Axis> YAxesChart1 { get; set; }
        public string[] GenerateLabels()
        {
            if (selectedChart1 == "week")
                // Triển khai logic tạo nhãn cho tuần
                return new string[] { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };
            else if (selectedChart1 == "month") return new string[] { "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12" };
            else return new string[] { "2023", "2024" };

        }


        /*For example if today is tuesday , the data of week just is monday and tuesday , 
         so that we should add zero values for remaining weekdays for consistency */
        private List<float> makeFull(List<float> valueSales, string types)
        {
            int max = types == "week" ? 7 : 12;

            for (int i = valueSales.Count; i < max; i++)
            {
                valueSales.Add(0f);
            }
            return valueSales;

        }

        private void LoadChart1()
        {
            List<float> valueSales;
            if (selectedChart1 == "week")
            {
                valueSales = Dao.getTotalSaleByWeek(DateTimeForWeek);
                valueSales = makeFull(valueSales, "week");
                Chart1 = new ISeries[]
                {
                new LineSeries<float>
                {
                    Values = valueSales,
                    Stroke = new SolidColorPaint(SKColors.Blue),
                    Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(50)),
                    Name = "Doanh số tuần"
                }
                };

            }
            else if (selectedChart1 == "month")
            {
                valueSales = Dao.getTotalSaleInMonth();
                valueSales = makeFull(valueSales, "month");

                Chart1 = new ISeries[]
                {
            new LineSeries<float>
            {
                Values = valueSales,
                Stroke = new SolidColorPaint(SKColors.Green),
                Fill = new SolidColorPaint(SKColors.Green.WithAlpha(50)),
                Name = "Doanh số tháng"
            }
                };


            }
            else
            {
                valueSales = Dao.getTotalSaleByYear();
                Chart1 = new ISeries[]
                {
            new LineSeries<float>
            {
                Values = valueSales,
                Stroke = new SolidColorPaint(SKColors.Red),
                Fill = new SolidColorPaint(SKColors.Red.WithAlpha(50)),
                Name = "Doanh số năm"
            }
                };
            }


            XAxesChart1 = new List<Axis>
                {
                    new Axis
                    {

                        Labels = GenerateLabels(),
                        LabelsRotation = 45,
                        LabelsPaint = new SolidColorPaint(SKColors.White),      // Màu trắng cho nhãn trục hoành
                        NamePaint = new SolidColorPaint(SKColors.White),        // Màu trắng cho tên trục hoành
                        SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                    }
                };
            YAxesChart1 = new List<Axis>
            {
                new() {
                    Name = "Doanh số (VNĐ)",
                    NamePaint = new SolidColorPaint(SKColors.White),       // Màu trắng cho tên trục tung
                    LabelsPaint = new SolidColorPaint(SKColors.White),     // Màu trắng cho nhãn trục tung
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                    MinLimit = 0,
                    MaxLimit = valueSales.Max() * 1.5f,
                    Labeler = value => value.ToString("N0")
                }
            };
        }


        public IEnumerable<ISeries> Chart2 { get; set; }
        public SKColor GenerateRandomColor()
        {
            Random random = new Random();
            return new SKColor(
                (byte)random.Next(256),
                (byte)random.Next(256),
                (byte)random.Next(256),
                200 // Độ trong suốt
            );
        }
        public LabelVisual TitleChart2 { get; set; } =
            new LabelVisual
            {
                Text = "Top Selling Items  ",
                Paint = new SolidColorPaint(SKColor.Parse("#FF5733")), // Màu cam đậm
                TextSize = 20,
                Padding = new LiveChartsCore.Drawing.Padding(0, 0, 0, 20), // Tăng khoảng cách dưới
                Y = 50,
            };
        public SolidColorPaint LegendTextPaint { get; set; } =
        new SolidColorPaint
        {
            Color = new SKColor(0, 0, 0),
            SKTypeface = SKTypeface.FromFamilyName("Segoe UI")
        };
        private void SetLegendTextPaint()
        {
            var currentTheme = Application.Current.RequestedTheme;

            // Kiểm tra chế độ Dark Mode hoặc Light Mode
            if (currentTheme == ApplicationTheme.Dark)
            {
                // Nếu hệ điều hành ở chế độ Dark Mode
                LegendTextPaint = new SolidColorPaint
                {
                    Color = new SKColor(255, 255, 255),  // Màu trắng cho Dark Mode
                    SKTypeface = SKTypeface.FromFamilyName("Segoe UI")
                };
            }
            else
            {
                // Nếu hệ điều hành ở chế độ Light Mode
                LegendTextPaint = new SolidColorPaint
                {
                    Color = new SKColor(0, 0, 0),  // Màu đen cho Light Mode
                    SKTypeface = SKTypeface.FromFamilyName("Segoe UI")
                };
            }
        }
        private void loadChart2()
        {
            List<KeyValuePair<string, float>> topSellingData;
            if (SelectedChart2 == "week")
            {
                topSellingData = Dao.getTopSellingByWeek();
                TitleChart2.Text = "Top Selling Items - Tuần";
            }
            else if (SelectedChart2 == "month")
            {
                topSellingData = Dao.getTopSellingByMonth();
                TitleChart2.Text = "Top Selling Items - Tháng";
            }
            else
            {
                topSellingData = Dao.getTopSellingByYear();
                TitleChart2.Text = "Top Selling Items - Năm";
            }

            float total = 0;
            topSellingData.ForEach((value) =>
            {
                total += value.Value;
            });
            Chart2 = topSellingData.Select(kvp =>
                new PieSeries<float>
                {
                    Values = new[] { kvp.Value * 100 / total },
                    Name = kvp.Key,
                    Fill = new SolidColorPaint(GenerateRandomColor()), // Thêm màu ngẫu nhiên
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue:N0} %", // Định dạng nhãn
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
                }).ToArray();

        }

        public List<Axis> XAxesChart3 { get; set; }
        public List<Axis> YAxesChart3 { get; set; }
        public LabelVisual TitleChart3 { get; set; } =
            new LabelVisual
            {
                Text = " Comparision Total Capital and Revenue",
                Paint = new SolidColorPaint(SKColor.Parse("#FF5733")), // Màu cam đậm
                TextSize = 20,
                Padding = new LiveChartsCore.Drawing.Padding(0, 0, 0, 20), // Tăng khoảng cách dưới
                Y = 50,
            };
        private void loadChart3()
        {
            List<KeyValuePair<float, float>> topSellingData;
            if (SelectedChart3 == "month")
            {
                topSellingData = Dao.getCompareCapitalWithValueByMonth();
                //TitleChart2.Text = "Top Selling Items - Tuần";
            }

            else
            {
                topSellingData = Dao.getCompareCapitalWithValueByYear();
                //TitleChart2.Text = "Top Selling Items - Năm";
            }


            Chart3 = new ISeries[]
            {
                new ColumnSeries<float>
                {
                    Name = "Capital",
                    DataLabelsPaint =  new SolidColorPaint(SKColors.White),
                    Values = new float[] { 2, 5, 4, 2, 3, 6 }
                },
                new ColumnSeries<float>
                {
                    Name = "Revenue",
                    DataLabelsPaint =  new SolidColorPaint(SKColors.White),
                    Values = new float[] { 3, 1, 6  , 3, 1, 6  }
                }
            };

            XAxesChart3 = new List<Axis>
                {
                    new Axis
                    {

                        Labels = GenerateLabels(),
                        LabelsRotation = 45,
                        LabelsPaint = new SolidColorPaint(SKColors.White),      // Màu trắng cho nhãn trục hoành
                        NamePaint = new SolidColorPaint(SKColors.White),        // Màu trắng cho tên trục hoành
                        SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                        SeparatorsAtCenter = false,
                        TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                        TicksAtCenter = true,
                        ForceStepToMin = true,
                        MinStep = 1

                    }
                };
            YAxesChart3 = new List<Axis>
            {
                new Axis
                {
                    Name = "Doanh số (VNĐ)",
                    NamePaint = new SolidColorPaint(SKColors.White),       // Màu trắng cho tên trục tung
                    LabelsPaint = new SolidColorPaint(SKColors.White),     // Màu trắng cho nhãn trục tung
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                    MinLimit = 0,
                    MaxLimit = 6 * 1.5f,
                    Labeler = value => value.ToString("N0")
                }
            };

        }

        public async void LoadModel()
        {
            try
            {
                var assetsFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
                var modelFile = await assetsFolder.GetFileAsync("SalesModel.zip");

                if (modelFile == null)
                {
                    Console.WriteLine("Model file not found.");
                    return;
                }

                LoadChart4And5(modelFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading model: {ex.Message}");
            }
        }

        public async void LoadChart4And5(StorageFile file)
        {
            var recommendedDrinks =
            await MlModelService.RecommendPopularDrinksAsync(GetOrderMenuItems(), DateTimeForTrainedData, _topProductsCount, file);

            if (recommendedDrinks.Any())
            {
                float total = recommendedDrinks.Sum(value => value.PredictedSales);
                var colors = recommendedDrinks.Select(_ => GenerateRandomColor()).ToArray();
                TitleChart4.Text = $"Top {_topProductsCount} Predicted Products";
                TopProductsSeries = recommendedDrinks.Select((p, index) =>
                    new PieSeries<float>
                    {
                        Values = new List<float> { p.PredictedSales * 100 / total },
                        Name = p.ProductName,
                        Fill = new SolidColorPaint(colors[index]),
                        DataLabelsPaint = new SolidColorPaint(SKColors.White),
                        DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue:N0} %",
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
                    }).ToArray<ISeries>();

                Chart5 = new ISeries[]
                {
                        new ColumnSeries<float>
                        {
                            Values = recommendedDrinks.Select(p => p.PredictedSales).ToList(),
                            Name = "Predicted Sales",
                            Fill = new SolidColorPaint(SKColors.Blue),
                            DataLabelsPaint = new SolidColorPaint(SKColors.White),
                            DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue:N0}",

                        }
                };

                // Configure the X-axis (Product Names)
                XAxesChart5 = new List<Axis>
                    {
                        new()
                        {
                            Labels = recommendedDrinks.Select(p => p.ProductName).ToArray(),
                            LabelsRotation = 15,
                            LabelsPaint = new SolidColorPaint(SKColors.Black),
                            TextSize = 14,
                            Name = "Products",
                            NamePaint = new SolidColorPaint(SKColors.Gray)
                        }
                    };

                // Configure the Y-axis (Predicted Sales)
                YAxesChart5 = new List<Axis>
                    {
                        new()
                        {
                            Name = "Predicted Sales",
                            NamePaint = new SolidColorPaint(SKColors.Gray),
                            TextSize = 14,
                            LabelsPaint = new SolidColorPaint(SKColors.Black),
                            Labeler = value => $"{value:N0}",
                            MinLimit = 0
                        }
                    };
            }
            else
            {
                Console.WriteLine("No recommendations available.");
            }
        }

        private DateTime _dateTimeForTrainedData;
        public DateTime DateTimeForTrainedData
        {
            set
            {
                if (_dateTimeForTrainedData != value)
                {
                    _dateTimeForTrainedData = value;
                }
            }
            get => _dateTimeForTrainedData;
        }

        public LabelVisual TitleChart4 { get; set; } =
            new LabelVisual
            {
                Text = "Top Predicted Products",
                Paint = new SolidColorPaint(SKColor.Parse("#FF5733")), // Màu cam đậm
                TextSize = 20,
                Padding = new LiveChartsCore.Drawing.Padding(0, 0, 0, 20), // Tăng khoảng cách dưới
                Y = 50,
            };

        public HashSet<OrderMenuItem> GetOrderMenuItems()
        {
            List<Order> list = Dao.GetOrders();
            IEnumerable<OrderItem> orderItems = list.SelectMany(order => order.OrderItems);
            var orderMenuItems = orderItems
                .Select(orderItem => orderItem.MenuItem)
                .GroupBy(menuItem => menuItem.Name)
                .Select(group => group.First())
                .ToHashSet();
            return orderMenuItems;
        }

        public ISeries[] Chart5 { get; set; }
        public List<Axis> XAxesChart5 { get; set; }
        public List<Axis> YAxesChart5 { get; set; }
    }
}
