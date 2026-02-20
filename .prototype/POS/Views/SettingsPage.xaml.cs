using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using POS.Services;
using POS.Services.Dao;
using POS.ViewModels;
using Windows.Storage;
using POS.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace POS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public RoleViewModel RoleViewModel;
        public SettingsViewModel SettingsViewModel;

        public SettingsPage()
        {
            this.InitializeComponent();
            RoleViewModel = App.GetService<RoleViewModel>();
            SettingsViewModel = App.GetService<SettingsViewModel>();
            if (RoleViewModel.CurrentRole == "Login")
                SettingsViewModel.IsLoggedOut = true;
            else
                SettingsViewModel.IsLoggedOut = false;
            DataContext = RoleViewModel;

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["id"] != null)
                SettingsViewModel.Id = Int32.Parse(localSettings.Values["id"].ToString());
            if (localSettings.Values["fullname"] != null)
                SettingsViewModel.Fullname = localSettings.Values["fullname"].ToString();
            if (localSettings.Values["username"] != null)
                SettingsViewModel.Username = localSettings.Values["username"].ToString();
            if (localSettings.Values["role"] != null)
                SettingsViewModel.Role = localSettings.Values["role"].ToString();

            if (localSettings.Values["DB_HOST"] == null)
            {
                localSettings.Values["DB_HOST"] = Config.GetSetting("DB_HOST");
                localSettings.Values["DB_PORT"] = Config.GetSetting("DB_PORT");
                localSettings.Values["DB_USER"] = Config.GetSetting("DB_USER");
                localSettings.Values["DB_PASSWORD"] = Config.GetSetting("DB_PASSWORD");
                localSettings.Values["DB_NAME"] = Config.GetSetting("DB_NAME");
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            RoleViewModel.UpdateRole("Login");
            SettingsViewModel.IsLoggedOut = true;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var loginDialog = new ContentDialog
            {
                Title = "Đăng nhập",
                Content = new StackPanel
                {
                    Children =
            {
                new TextBox { PlaceholderText = "Tên đăng nhập", Name = "UsernameTextBox", Margin = new Thickness(0, 16, 0, 16) },
                new PasswordBox { PlaceholderText = "Mật khẩu", Name = "PasswordBox" }
            }
                },
                PrimaryButtonText = "Đăng nhập",
                CloseButtonText = "Thoát"
            };

            loginDialog.XamlRoot = this.Content.XamlRoot;

            // Hiển thị ContentDialog và chờ kết quả người dùng
            var result = await loginDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Lấy StackPanel từ ContentDialog và tìm các điều khiển
                var stackPanel = (StackPanel)loginDialog.Content;

                // Tìm các điều khiển TextBox và PasswordBox trong StackPanel
                var usernameTextBox = stackPanel.Children.OfType<TextBox>().FirstOrDefault();
                var passwordBox = stackPanel.Children.OfType<PasswordBox>().FirstOrDefault();

                // Lấy giá trị tên đăng nhập và mật khẩu
                var username = usernameTextBox?.Text ?? "";
                var password = passwordBox?.Password ?? "";

                // Kiểm tra nếu tên đăng nhập hoặc mật khẩu trống
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Lỗi",
                        Content = "Tên đăng nhập và mật khẩu không được để trống.",
                        CloseButtonText = "OK"
                    };
                    errorDialog.XamlRoot = this.Content.XamlRoot;
                    await errorDialog.ShowAsync();
                }
                else
                {
                    var db = new DatabaseManager();
                    try
                    {
                        db.Connect();
                    }
                    catch (Exception ex)
                    {
                        var errorDialog = new ContentDialog
                        {
                            Title = "Lỗi",
                            Content = "Không thể kết nối với Database.",
                            CloseButtonText = "OK"
                        };
                        errorDialog.XamlRoot = this.Content.XamlRoot;
                        await errorDialog.ShowAsync();
                        return;
                    }

                    try
                    {
                        db.Command.CommandText = "SELECT * FROM employee WHERE username = @username AND password = @password";
                        db.Command.Parameters.AddWithValue("username", username);
                        db.Command.Parameters.AddWithValue("password", password);
                        var reader = await db.Command.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            db.Disconnect();

                            db.Connect();
                            db.Command.CommandText = "SELECT role FROM employee WHERE username = @username AND password = @password";
                            db.Command.Parameters.AddWithValue("username", username);
                            db.Command.Parameters.AddWithValue("password", password);
                            var role = await db.Command.ExecuteScalarAsync();
                            db.Disconnect();

                            db.Connect();
                            db.Command.CommandText = "SELECT fullname FROM employee WHERE username = @username AND password = @password";
                            db.Command.Parameters.AddWithValue("username", username);
                            db.Command.Parameters.AddWithValue("password", password);
                            var fullname = await db.Command.ExecuteScalarAsync();
                            db.Disconnect();

                            db.Connect();
                            db.Command.CommandText = "SELECT id FROM employee WHERE username = @username AND password = @password";
                            db.Command.Parameters.AddWithValue("username", username);
                            db.Command.Parameters.AddWithValue("password", password);
                            var id = await db.Command.ExecuteScalarAsync();
                            db.Disconnect();

                            SettingsViewModel.Id = (int)(id ?? "");
                            SettingsViewModel.Fullname = (string)(fullname ?? "");
                            SettingsViewModel.Username = (string)(username ?? "");
                            SettingsViewModel.Role = (string)(role ?? "");

                            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                            localSettings.Values["id"] = SettingsViewModel.Id;
                            localSettings.Values["fullname"] = SettingsViewModel.Fullname;
                            localSettings.Values["username"] = SettingsViewModel.Username;
                            localSettings.Values["role"] = SettingsViewModel.Role;

                            if (role?.ToString() == "Manager")
                            {
                                RoleViewModel.UpdateRole("Manager");
                                CurrentUser.setCurrent(username);
                                SettingsViewModel.IsLoggedOut = false;
                            }
                            else
                            {
                                RoleViewModel.UpdateRole("Cashier");
                                CurrentUser.setCurrent(username);
                                SettingsViewModel.IsLoggedOut = false;
                            }
                        }
                        else
                        {
                            var errorDialog = new ContentDialog
                            {
                                Title = "Lỗi",
                                Content = "Tên đăng nhập hoặc mật khẩu không đúng.",
                                CloseButtonText = "OK"
                            };
                            errorDialog.XamlRoot = this.Content.XamlRoot;
                            await errorDialog.ShowAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorDialog = new ContentDialog
                        {
                            Title = "Lỗi",
                            Content = ex.Message,
                            CloseButtonText = "OK"
                        };
                        errorDialog.XamlRoot = this.Content.XamlRoot;
                        await errorDialog.ShowAsync();
                    }
                }
            }
        }

        private void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                string selectedContent = selectedItem.Content.ToString();
                var settings = ApplicationData.Current.LocalSettings;

                switch (selectedContent)
                {
                    case "Sáng":
                        settings.Values["Theme"] = "Light";
                        break;
                    case "Tối":
                        settings.Values["Theme"] = "Dark";
                        break;
                    case "Tự động":
                        settings.Values["Theme"] = "Auto";
                        break;
                }

                // Đang dang dờ :<
                // App.xaml.cs
                App.SetAppTheme();
            }
        }

        private async void Repository_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://github.com/nguyen-anh-hao/POS/");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void DatabaseConfig_Click(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            var configDialog = new ContentDialog
            {
                Title = "Cấu hình Database",
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBox { PlaceholderText = "Host", Name = "DB_HOST", Margin = new Thickness(0, 16, 0, 16), Text = localSettings.Values["DB_HOST"].ToString() },
                        new TextBox { PlaceholderText = "Port", Name = "DB_PORT", Margin = new Thickness(0, 16, 0, 16), Text = localSettings.Values["DB_PORT"].ToString() },
                        new TextBox { PlaceholderText = "Username", Name = "DB_USER", Margin = new Thickness(0, 16, 0, 16), Text = localSettings.Values["DB_USER"].ToString() },
                        new PasswordBox { PlaceholderText = "Password", Name = "DB_PASSWORD", Margin = new Thickness(0, 16, 0, 16) },
                        new TextBox { PlaceholderText = "Database name", Name = "DB_NAME", Margin = new Thickness(0, 16, 0, 16), Text = localSettings.Values["DB_NAME"].ToString() }
                    }
                },
                PrimaryButtonText = "Lưu",
                CloseButtonText = "Thoát"
            };

            configDialog.XamlRoot = this.Content.XamlRoot;
            var result = await configDialog.ShowAsync();

            // Lấy StackPanel từ ContentDialog và tìm các điều khiển
            var stackPanel = (StackPanel)configDialog.Content;

            // Tìm các điều khiển TextBox và PasswordBox trong StackPanel
            var dbHost = stackPanel.Children.OfType<TextBox>().FirstOrDefault(x => x.Name == "DB_HOST");
            var dbPort = stackPanel.Children.OfType<TextBox>().FirstOrDefault(x => x.Name == "DB_PORT");
            var dbUser = stackPanel.Children.OfType<TextBox>().FirstOrDefault(x => x.Name == "DB_USER");
            var dbPassword = stackPanel.Children.OfType<PasswordBox>().FirstOrDefault(x => x.Name == "DB_PASSWORD");
            var dbName = stackPanel.Children.OfType<TextBox>().FirstOrDefault(x => x.Name == "DB_NAME");

            // Lấy giá trị tên đăng nhập và mật khẩu
            var host = dbHost?.Text ?? "";
            var port = dbPort?.Text ?? "";
            var user = dbUser?.Text ?? "";
            var password = dbPassword?.Password ?? "";
            var name = dbName?.Text ?? "";


            if (result == ContentDialogResult.Primary)
            {
                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name))
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Lỗi",
                        Content = "Cấu hình không được để trống.",
                        CloseButtonText = "OK"
                    };
                    errorDialog.XamlRoot = this.Content.XamlRoot;
                    await errorDialog.ShowAsync();
                }
                else
                {
                    var settings = ApplicationData.Current.LocalSettings;
                    settings.Values["DB_HOST"] = host;
                    settings.Values["DB_PORT"] = port;
                    settings.Values["DB_USER"] = user;
                    settings.Values["DB_PASSWORD"] = password;
                    settings.Values["DB_NAME"] = name;
                    var successDialog = new ContentDialog
                    {
                        Title = "Thành công",
                        Content = "Cấu hình đã được lưu.",
                        CloseButtonText = "OK"
                    };
                    successDialog.XamlRoot = this.Content.XamlRoot;
                    await successDialog.ShowAsync();
                }
            }
        }

        private void PaymentConfig_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
//            RoleViewModel.UpdateRole("Manager");
//            CurrentUser.setCurrent(UsernameTextBox.Text);  
//            SettingsViewModel.IsLoggedOut = false;
//        }

//        private void Cashier_Click(object sender, RoutedEventArgs e)
//        {
//            RoleViewModel.UpdateRole("Cashier");
//            CurrentUser.setCurrent(UsernameTextBox.Text);
//            SettingsViewModel.IsLoggedOut = false;
//        }
//    }
//}
