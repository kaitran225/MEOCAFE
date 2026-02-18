using Npgsql;
using POS.Models;
using System.Collections.ObjectModel;
using NpgsqlTypes;
using static SkiaSharp.HarfBuzz.SKShaper;
using POS.Views.EmployeeViews;

namespace POS.Services.Dao
{
    public class PostgreSqlDao : IDao
    {
        private DatabaseManager db = new DatabaseManager();
        private readonly string ConnectionString;
        string connectionString;
        private readonly NpgsqlConnection _connection;

        public PostgreSqlDao()
        {
            ConnectionString = db.GetConnectionString();
            connectionString = db.GetConnectionString();
            _connection = new NpgsqlConnection(ConnectionString);
            _connection.Open();
        }

        public void CloseConnection()
        {
            _connection.Close();
        }

        public List<Category> GetCategories()
        {
            var categories = new List<Category>();

            string query = "SELECT * FROM categories ORDER BY id ASC ";
            using var command = new NpgsqlCommand(query, _connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var category = new Category
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        MenuItems = new List<MenuItem>()
                    };
                    categories.Add(category);
                }
            }

            return categories;
        }

        public Category? GetCategory(int id)
        {
            Category category = null;  // Start with null to return if no data is found

            string query = "SELECT * FROM categories WHERE id = @Id";


            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read()) // Check if any row is returned
                    {
                        category = new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                        };
                    }
                }
            }
            return category;  // Will return null if no menu item is found
        }

        public List<MenuItem> GetMenuItems()
        {
            var menuItems = new List<MenuItem>();

            string query = "SELECT * FROM menu_items ORDER BY id ASC ";
            using (var command = new NpgsqlCommand(query, _connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var menuItem = new MenuItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        SellPrice = reader.GetDecimal(2),
                        CapitalPrice = reader.GetDecimal(3),
                        Quantity = reader.GetInt32(4),
                        Image = reader["image"] as byte[],
                        CategoryId = reader.GetInt32(6)
                    };
                    menuItems.Add(menuItem);
                }
            }

            return menuItems;
        }

        public MenuItem GetMenuItem(int id)
        {
            MenuItem menuItem = null;  // Start with null to return if no data is found

            string query = "SELECT * FROM menu_items WHERE id = @Id";


            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read()) // Check if any row is returned
                    {
                        menuItem = new MenuItem
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            SellPrice = reader.GetDecimal(2),
                            CapitalPrice = reader.GetDecimal(3),
                            Quantity = reader.GetInt32(4),
                            // Handle possible nulls in the "image" column
                            Image = reader["image"] as byte[], // This will return null if the column is DBNull
                            CategoryId = reader.GetInt32(6),
                        };
                    }
                }
            }

            return menuItem;  // Will return null if no menu item is found
        }

        public List<OrderItem> GetOrderItems()
        {
            // Implement order item fetching if needed
            return new List<OrderItem>();
        }

        public List<ComboMenuItem> GetComboMenuItems()
        {
            var comboMenuItems = new List<ComboMenuItem>();

            string query = "SELECT * FROM combo_menu_items ORDER BY id ASC ";
            using var command = new NpgsqlCommand(query, _connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var comboMenuItem = new ComboMenuItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        SellPrice = reader.GetDecimal(2),
                        Description = reader.GetString(3),
                        Image = reader["image"] as byte[],
                        Quantity = reader.GetInt32(5),
                    };
                    comboMenuItems.Add(comboMenuItem);
                }
            }

            return comboMenuItems;
        }

        public List<ComboItem> GetComboItemsByComboMenuItemsId(int id)
        {
            List<ComboItem> comboItems = new();

            string query = "SELECT * FROM combo_items WHERE combo_id = @Id ORDER BY id ASC ";


            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var comboItem = new ComboItem
                        {
                            Id = reader.GetInt32(0),
                            ComboMenuItemId = reader.GetInt32(1),
                            MenuItemId = reader.GetInt32(2),
                        };
                        comboItems.Add(comboItem);
                    }
                }
            }

            return comboItems;  // Will return null if no menu item is found
        }

        public List<ComboItem> GetComboItems()
        {
            var comboItems = new List<ComboItem>();

            string query = "SELECT * FROM combo_items ORDER BY id ASC ";
            using var command = new NpgsqlCommand(query, _connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var comboItem = new ComboItem
                    {
                        Id = reader.GetInt32(0),
                        ComboMenuItemId = reader.GetInt32(1),
                        MenuItemId = reader.GetInt32(2),
                    };
                    comboItems.Add(comboItem);
                }
            }

            return comboItems;
        }

        public List<Discount> GetDiscounts()
        {
            var discounts = new List<Discount>();

            string query = "SELECT * FROM discounts ORDER BY id ASC ";
            using var command = new NpgsqlCommand(query, _connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var discount = new Discount
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        Percentage = reader.GetDecimal(3),
                        IsDisabled = reader.GetBoolean(4),
                        StartDate = reader.GetDateTime(5),
                        EndDate = reader.GetDateTime(6)
                    };
                    discounts.Add(discount);
                }
            }

            return discounts;
        }

        public Discount? GetDiscount(int menuItemId)
        {
            Discount discount = null;  // Start with null to return if no data is found

            string query = "SELECT d.id, d.name, d.description, d.percentage, d.is_disabled, d.start_date, d.end_date " +
                           "FROM menu_item_discount mid " +
                           "JOIN discounts d ON mid.discount_id = d.id " +
                           "WHERE mid.menu_item_id = @Id";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Id", menuItemId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read()) // Check if any row is returned
                    {
                        discount = new Discount
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Percentage = reader.GetDecimal(3),
                            IsDisabled = reader.GetBoolean(4),
                            StartDate = reader.GetDateTime(5),
                            EndDate = reader.GetDateTime(6)
                        };
                    }
                }
            }

            return discount;
        }

        public async Task AddCategoryAsync(Category category)
        {
            string query = "INSERT INTO categories (name) VALUES (@Name) RETURNING id;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Name", category.Name);
                category.Id = (int)await command.ExecuteScalarAsync();
            }
        }

public async Task AddMenuItemAsync(MenuItem menuItem)
{
    string query = "INSERT INTO menu_items (name, \"sell_price\", \"capital_price\", quantity, image, \"category_id\") " +
                   "VALUES (@Name, @SellPrice, @CapitalPrice, @Quantity, @Image, @CategoryId) RETURNING id;";

    await using (var connection = new NpgsqlConnection(connectionString)) // Use a fresh connection
    {
        await connection.OpenAsync();

        await using (var command = new NpgsqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Name", menuItem.Name);
            command.Parameters.AddWithValue("@SellPrice", menuItem.SellPrice);
            command.Parameters.AddWithValue("@CapitalPrice", menuItem.CapitalPrice);
            command.Parameters.AddWithValue("@Quantity", menuItem.Quantity);
            command.Parameters.AddWithValue("@Image", (object)menuItem.Image ?? DBNull.Value);
            command.Parameters.AddWithValue("@CategoryId", menuItem.CategoryId);

            menuItem.Id = (int)await command.ExecuteScalarAsync();
        }
    }
}


        public async Task AddComboMenuItemAsync(ComboMenuItem comboMenuItem)
        {
            string query = "INSERT INTO combo_menu_items (name, \"combo_price\", \"description\", quantity, image) " +
                           "VALUES (@Name, @SellPrice, @Description, @Quantity, @Image) RETURNING id;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Name", comboMenuItem.Name);
                command.Parameters.AddWithValue("@SellPrice", comboMenuItem.SellPrice);
                command.Parameters.AddWithValue("@Description", comboMenuItem.Description);
                command.Parameters.AddWithValue("@Quantity", comboMenuItem.Quantity);
                command.Parameters.AddWithValue("@Image", (object)comboMenuItem.Image ?? DBNull.Value);

                comboMenuItem.Id = (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task AddComboItemAsync(ComboItem comboItem)
        {
            string query = "INSERT INTO combo_items (\"combo_id\", \"menu_item_id\") " +
                           "VALUES (@ComboId, @MenuItemId)";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@ComboId", comboItem.ComboMenuItemId);
                command.Parameters.AddWithValue("@MenuItemId", comboItem.MenuItemId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddDiscountAsync(Discount discount)
        {
            string query = "INSERT INTO discounts (\"name\", \"description\", \"percentage\", \"start_date\", \"end_date\") " +
                           "VALUES (@Name, @Description, @Percentage, @StartDate, @EndDate) RETURNING id;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Name", discount.Name);
                command.Parameters.AddWithValue("@Description", discount.Description);
                command.Parameters.AddWithValue("@Percentage", discount.Percentage);
                command.Parameters.AddWithValue("@StartDate", discount.StartDate);
                command.Parameters.AddWithValue("@EndDate", discount.EndDate);

                discount.Id = (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task AddMenuItemDiscountAsync(int discountId, int menuItemId)
        {
            string query = " INSERT INTO menu_item_discount (\"menu_item_id\", \"discount_id\") " +
            " VALUES(@MenuItemId, @DiscountId)" +
            " ON CONFLICT(\"menu_item_id\") " +
            " DO UPDATE SET \"discount_id\" = EXCLUDED.\"discount_id\";";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@MenuItemId", menuItemId);
                command.Parameters.AddWithValue("@DiscountId", discountId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task EditMenuItemAsync(MenuItem menuItem)
        {
            string query = "UPDATE menu_items SET name = @Name, \"sell_price\" = @SellPrice, \"capital_price\" = @CapitalPrice, " +
                           "quantity = @Quantity, image = @Image WHERE id = @Id;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Name", menuItem.Name);
                command.Parameters.AddWithValue("@SellPrice", menuItem.SellPrice);
                command.Parameters.AddWithValue("@CapitalPrice", menuItem.CapitalPrice);
                command.Parameters.AddWithValue("@Quantity", menuItem.Quantity);
                command.Parameters.AddWithValue("@Image", (object)menuItem.Image ?? DBNull.Value);
                command.Parameters.AddWithValue("@Id", menuItem.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task EditComboMenuItemAsync(ComboMenuItem comboMenuItem)
        {
            string query = "UPDATE combo_menu_items SET name = @Name, \"combo_price\" = @SellPrice, \"description\" = @Description, " +
                           "quantity = @Quantity, image = @Image WHERE id = @Id;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Name", comboMenuItem.Name);
                command.Parameters.AddWithValue("@SellPrice", comboMenuItem.SellPrice);
                command.Parameters.AddWithValue("@Description", comboMenuItem.Description);
                command.Parameters.AddWithValue("@Quantity", comboMenuItem.Quantity);
                command.Parameters.AddWithValue("@Image", (object)comboMenuItem.Image ?? DBNull.Value);
                command.Parameters.AddWithValue("@Id", comboMenuItem.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task EditDiscountAsync(Discount discount)
        {
            string query = "UPDATE discounts SET name = @Name, description = @Description, percentage = @Percentage, " +
                           "\"is_disabled\" = @IsDisabled, start_date = @StartDate, end_date = @EndDate WHERE id = @Id;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Name", discount.Name);
                command.Parameters.AddWithValue("@Description", discount.Description);
                command.Parameters.AddWithValue("@Percentage", discount.Percentage);
                command.Parameters.AddWithValue("@IsDisabled", discount.IsDisabled);
                command.Parameters.AddWithValue("@StartDate", discount.StartDate);
                command.Parameters.AddWithValue("@EndDate", discount.EndDate);
                command.Parameters.AddWithValue("@Id", discount.Id);

                await command.ExecuteNonQueryAsync();
            }
        }


        public async Task DeleteCategoryAsync(int categoryId)
        {
            string query = "DELETE FROM categories WHERE id = @CategoryId;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@CategoryId", categoryId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteMenuItemAsync(int menuItemId)
        {
            string query = "DELETE FROM menu_items WHERE id = @MenuItemId;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@MenuItemId", menuItemId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteComboMenuItemAsync(int comboMenuItemId)
        {
            string query = "DELETE FROM combo_menu_items WHERE id = @ComboMenuItemId;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@ComboMenuItemId", comboMenuItemId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteComboItemAsync(int comboMenuItemId, int menuItemId)
        {
            string query = "DELETE FROM combo_items WHERE combo_id = @ComboItemId AND menu_item_id = @MenuItemId;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@ComboItemId", comboMenuItemId);
                command.Parameters.AddWithValue("@MenuItemId", menuItemId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteComboItemAsync(int comboMenuItemId)
        {
            string query = "DELETE FROM combo_items WHERE combo_id = @ComboItemId;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@ComboItemId", comboMenuItemId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteDiscountAsync(int discountId)
        {
            string query = "DELETE FROM discounts WHERE id = @DiscountId;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@DiscountId", discountId);
                await command.ExecuteNonQueryAsync();
            }
        }

        private ObservableCollection<Employee> employees;

        private ObservableCollection<Customer> customers = new ObservableCollection<Customer>
        {
            new ()
                {
                    Fullname = "Jose Murphy" ,
                    PhoneNumber = "1234567890",
                    Point = ""
                },

        };

        //public List<OrderItem> GetOrderItems()
        //{
        //    List<Category> categories = GetCategories();

        //    Category cafe = categories[0];
        //    Category cake = categories[1];

        //    List<MenuItem> menuItemsOfCafe = cafe.MenuItems;
        //    List<MenuItem> menuItemsOfCake = cake.MenuItems;

        //    return new List<OrderItem>
        //    {
        //        new OrderItem { MenuItem = menuItemsOfCafe[0], Quantity = 1 },
        //        new OrderItem { MenuItem = menuItemsOfCafe[4], Quantity = 2 },
        //        new OrderItem { MenuItem = menuItemsOfCake[0], Quantity = 3 },
        //    };
        //}



        /*Employee*/

        public ObservableCollection<Employee> GetEmployees()
        {
            ObservableCollection<Employee> employees = new ObservableCollection<Employee>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand("SELECT id, fullname, username, password, phone_number, gender, address, dob, role FROM employee", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Employee employee = new Employee
                                {
                                    ID = reader["id"].ToString(),
                                    Fullname = reader["fullname"].ToString(),
                                    Username = reader["username"].ToString(),
                                    Password = reader["password"].ToString(),
                                    PhoneNumber = reader["phone_number"].ToString(),
                                    Gender = reader["gender"].ToString(),
                                    Address = reader["address"].ToString(),
                                    Dob = reader["dob"].ToString(),
                                    Role = reader["role"].ToString()
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }

            return employees;
        }
        public bool AddEmployee(Employee newEmployee)
        {

            string query = @"INSERT INTO employee (fullname, username, password, phone_number, gender, address, dob, role)
                     VALUES ( @Fullname, @Username, @Password, @PhoneNumber, @Gender, @Address, @Dob, @Role)";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Fullname", newEmployee.Fullname);
                        command.Parameters.AddWithValue("@Username", newEmployee.Username);
                        command.Parameters.AddWithValue("@Password", newEmployee.Password);
                        command.Parameters.AddWithValue("@PhoneNumber", newEmployee.PhoneNumber);
                        command.Parameters.AddWithValue("@Gender", newEmployee.Gender);
                        command.Parameters.AddWithValue("@Address", newEmployee.Address);
                        command.Parameters.AddWithValue("@Dob", DateTime.Parse(newEmployee.Dob)); // Đảm bảo định dạng ngày tháng đúng
                        command.Parameters.AddWithValue("@Role", newEmployee.Role);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }
        public bool DeleteEmployee(string id)
        {

            string query = @"DELETE FROM employee
                    WHERE ID = @Id ";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", int.Parse(id));


                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }



        public bool UpdateEmployee(int id, Employee newEmployee)
        {
            string query = @"UPDATE employee
                            SET fullname = @fullname,
                             username = @username,
                             phone_number = @phone_number,
                             gender = @gender,
                             address = @address,
                             dob = @dob,
                                role = @role
                                WHERE id = @id";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@fullname", newEmployee.Fullname);
                        command.Parameters.AddWithValue("@username", newEmployee.Username);
                        command.Parameters.AddWithValue("@phone_number", newEmployee.PhoneNumber);
                        command.Parameters.AddWithValue("@gender", newEmployee.Gender);
                        command.Parameters.AddWithValue("@address", newEmployee.Address);
                        command.Parameters.AddWithValue("@dob", DateTime.Parse(newEmployee.Dob));
                        command.Parameters.AddWithValue("@role", newEmployee.Role);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }

        /*For stactistics */
        /*equivalence with final_price trong Orders */


        public List<float> getTotalSaleByWeek(DateTime referenceDate)
        {
            var weeklySales = new List<float>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                var query = @"
        WITH week_days AS (
            SELECT
                generate_series(
                    -- Xác định ngày bắt đầu tuần
                    CASE
                        WHEN EXTRACT(DOW FROM @reference_date) = 1 THEN @reference_date::DATE - INTERVAL '6 days'
                        ELSE @reference_date::DATE - INTERVAL '1 day' * (EXTRACT(DOW FROM @reference_date) - 1)
                    END,
                    -- Ngày kết thúc tuần
                    CASE
                        WHEN EXTRACT(DOW FROM @reference_date) = 1 THEN @reference_date::DATE
                        ELSE @reference_date::DATE + INTERVAL '1 day' * (7 - EXTRACT(DOW FROM @reference_date))
                    END,
                    '1 day'
                ) AS sale_date
        )
        SELECT
            wd.sale_date,
            COALESCE(SUM(o.grand_total), 0) AS daily_sales
        FROM
            week_days wd
        LEFT JOIN Orders o ON DATE(o.created_at) = wd.sale_date
        GROUP BY wd.sale_date
        ORDER BY wd.sale_date;";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    // Thêm tham số `@reference_date` vào truy vấn
                    command.Parameters.AddWithValue("@reference_date", referenceDate);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            float dailySales = Convert.ToSingle(reader.GetDouble(1));
                            weeklySales.Add(dailySales);
                        }
                    }
                }
            }

            return weeklySales;
        }
        public List<float> getTotalSaleInMonth()
        {
            var monthlySales = new List<float>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                var query = @"
        SELECT
            EXTRACT(MONTH FROM o.created_at) AS month,
            COALESCE(SUM(o.grand_total), 0) AS total_sales
        FROM
            Orders o
        WHERE
            EXTRACT(YEAR FROM o.created_at) = EXTRACT(YEAR FROM CURRENT_DATE) -- Lọc theo năm hiện tại
        GROUP BY
            EXTRACT(MONTH FROM o.created_at)
        ORDER BY
            month;";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Đọc tổng doanh thu cho mỗi tháng
                            float monthlySale = Convert.ToSingle(reader.GetDouble(1));
                            monthlySales.Add(monthlySale);
                        }
                    }
                }
            }

            return monthlySales;
        }
        public List<float> getTotalSaleByYear()
        {
            var yearlySales = new List<float>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                var query = @"
                    SELECT
                        EXTRACT(YEAR FROM o.created_at) AS year,
                        COALESCE(SUM(o.grand_total), 0) AS total_sales
                    FROM
                        Orders o
                    GROUP BY
                        EXTRACT(YEAR FROM o.created_at)
                    ORDER BY
                        year;";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Đọc tổng doanh thu cho mỗi năm
                            float yearlySale = Convert.ToSingle(reader.GetDouble(1));
                            yearlySales.Add(yearlySale);
                        }
                    }
                }
            }

            return yearlySales;
        }


        public List<KeyValuePair<string, float>> getTopSellingByWeek()
        {
            Random random = new Random();
            List<KeyValuePair<string, float>> topSelling = new List<KeyValuePair<string, float>>();

            string[] items = new string[]
            {
            "Nước cam",
            "Cappuccino",
            "Cà phê",
            "Trà sữa",
            "Bánh ngọt",
            "Sinh tố",
            "Latte",
            "Espresso",
            "Nước dừa",
            "Bánh mì"
            };

            foreach (var item in items)
            {
                float sale = (float)(random.Next(300000, 1000001)); // Tạo số ngẫu nhiên trong khoảng từ 300,000 đến 1,000,000
                topSelling.Add(new KeyValuePair<string, float>(item, sale));
            }

            return topSelling;
        }
        public List<KeyValuePair<string, float>> getTopSellingByMonth()
        {
            Random random = new Random();
            List<KeyValuePair<string, float>> topSelling = new List<KeyValuePair<string, float>>();

            string[] items = new string[]
            {
            "Nước cam",
            "Cappuccino",
            "Cà phê",
            "Trà sữa",
            "Bánh ngọt",
            "Sinh tố",
            "Latte",
            "Espresso",
            "Nước dừa",
            "Bánh mì"
            };

            foreach (var item in items)
            {
                float sale = (float)(random.Next(9000000, 30000000));
                topSelling.Add(new KeyValuePair<string, float>(item, sale));
            }

            return topSelling;
        }
        public List<KeyValuePair<string, float>> getTopSellingByYear()
        {
            Random random = new Random();
            List<KeyValuePair<string, float>> topSelling = new List<KeyValuePair<string, float>>();

            string[] items = new string[]
            {
            "Nước cam",
            "Cappuccino",
            "Cà phê",
            "Trà sữa",
            "Bánh ngọt",
            "Sinh tố",
            "Latte",
            "Espresso",
            "Nước dừa",
            "Bánh mì"
            };

            foreach (var item in items)
            {
                float sale = (float)(random.Next(30000000, 100000001));
                topSelling.Add(new KeyValuePair<string, float>(item, sale));
            }

            return topSelling;
        }
        /*For customer */

        public bool DeleteCustomer(string telephoneNumber)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    var query = "DELETE FROM customer WHERE phone_number = @phone_number";
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@phone_number", telephoneNumber);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception (optional)
                return false;
            }
        }
        public bool AddCustomer(Customer customer)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO customer (name, phone_number, point) VALUES (@name, @phone_number, @point)";
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", customer.Fullname);
                        command.Parameters.AddWithValue("@phone_number", customer.PhoneNumber);
                        command.Parameters.AddWithValue("@point", customer.Point);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool UpdateCustomer(Customer customer)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    var query = "UPDATE customer SET name = @name, point = @point WHERE phone_number = @phone_number";
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", customer.Fullname);
                        command.Parameters.AddWithValue("@phone_number", customer.PhoneNumber);
                        command.Parameters.AddWithValue("@point", customer.Point);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception (optional)
                return false;
            }
        }
        public bool CustomerAddPoint(string telephoneNumber, float point)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    var query = "UPDATE customer SET point = point + @point WHERE phone_number = @phone_number";
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@phone_number", telephoneNumber);
                        command.Parameters.AddWithValue("@point", point);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public List<KeyValuePair<float, float>> getCompareCapitalWithValueByMonth()
        {
            List<KeyValuePair<float, float>> a = new List<KeyValuePair<float, float>>();
            return a;
        }
        public List<KeyValuePair<float, float>> getCompareCapitalWithValueByYear()
        {
            List<KeyValuePair<float, float>> a = new List<KeyValuePair<float, float>>();
            return a;
        }


        public float getTotalSales()
        {
            float totalSales = 0;

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                var query = @"
        SELECT COALESCE(SUM(grand_total), 0) AS total_sales
        FROM Orders
        WHERE EXTRACT(YEAR FROM created_at) = EXTRACT(YEAR FROM NOW());
        ";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totalSales = Convert.ToSingle(reader["total_sales"]);
                        }
                    }
                }
            }

            return totalSales;
        }

        public float AverageRevenuePerTransaction()
        {
            float averageRevenue = 0;

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                var query = @"
            SELECT COALESCE(AVG(grand_total), 0) AS average_revenue
            FROM Orders;
            ";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            averageRevenue = Convert.ToSingle(reader["average_revenue"]);
                        }
                    }
                }
            }

            return averageRevenue;
        }

        public float CustomerReturning()
        {
            float returningCustomerRatio = 0;

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                var query = @"
        SELECT 
            COALESCE(
                CAST(COUNT(DISTINCT phone_number) FILTER (WHERE order_count > 1) AS FLOAT) /
                NULLIF(COUNT(DISTINCT phone_number), 0),
            0) AS returning_customer_ratio
        FROM (
            SELECT phone_number, COUNT(*) AS order_count
            FROM Orders
            WHERE phone_number IS NOT NULL
            GROUP BY phone_number
        ) subquery;
        ";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            returningCustomerRatio = Convert.ToSingle(reader["returning_customer_ratio"]);
                        }
                    }
                }
            }

            return returningCustomerRatio;
        }

        public KeyValuePair<string, float> getTopSelling()
        {
            string productName = null;
            float totalSales = 0;

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                var query = @"
                    SELECT 
                        mi.name AS menu_item_name, 
                        SUM(od.price) AS total_sales
                    FROM 
                        order_details od
                    JOIN 
                        Orders o ON od.order_id = o.id
                    JOIN 
                        menu_items mi ON od.menu_item_id = mi.id
                    WHERE 
                        DATE_PART('year', o.created_at) = DATE_PART('year', CURRENT_DATE)
                    GROUP BY 
                        mi.name
                    ORDER BY 
                        total_sales DESC
                    LIMIT 1;
                ";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            productName = reader["menu_item_name"].ToString();
                            totalSales = Convert.ToSingle(reader["total_sales"]);
                        }
                    }
                }
            }

            return new KeyValuePair<string, float>(productName, totalSales);
        }

        public async Task DeleteMenuItemDiscountAsync(int menuItemId)
        {
            string query = "DELETE FROM menu_item_discount WHERE menu_item_id = @MenuItemId;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@MenuItemId", menuItemId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public List<Item> GetComboItems(int comboId)
        {
            string query = @"
                SELECT mi.id, mi.name, mi.sell_price, mi.quantity, mi.image 
                FROM combo_items ci
                JOIN menu_items mi ON ci.menu_item_id = mi.id
                WHERE ci.combo_id = @ComboId;";

            List<Item> comboItems = new List<Item>();

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@ComboId", comboId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Item
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            SellPrice = reader.GetDecimal(2),
                            Quantity = reader.GetInt32(3),
                            Image = reader["image"] as byte[]
                        };
                        comboItems.Add(item);
                    }
                }
            }

            return comboItems;
        }

        public List<Order> GetOrders()
        {
            var orders = new List<Order>();

            string query = "SELECT * FROM orders ORDER BY id ASC";
            using var command = new NpgsqlCommand(query, _connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var _id = reader.GetValue(0);
                    var _totalPrice = reader.GetValue(1);
                    //var _percentage = reader.GetValue(2) ?? null;
                    var _grandTotal = reader.GetValue(3);
                    var _employeeId = reader.GetValue(4);
                    var _note = reader.GetValue(5);
                    var _phoneNumber = reader.GetValue(6);
                    var _createdAt = reader.GetValue(7);

                    var _dao = new PostgreSqlDao();
                    var _orderItems = _dao.GetOrderItemsByOrderId(Int32.Parse(_id?.ToString() ?? "0"));
                    var _employee = _dao.GetEmployee(Int32.Parse(_employeeId?.ToString() ?? "0"));
                    _dao.CloseConnection();

                    var _Id = Int32.Parse(_id?.ToString() ?? "0");
                    var _TotalPrice = Decimal.Parse(_totalPrice?.ToString() ?? "0");
                    decimal _Percentage = 0;
                    //if (_percentage != null)
                    //{
                    //_Percentage = Decimal.Parse(_percentage?.ToString() ?? "0");
                    //}
                    var _GrandTotal = Decimal.Parse(_grandTotal?.ToString() ?? "0");
                    var _Employee = _employee;
                    var _Note = _note?.ToString() ?? "";
                    var _PhoneNumber = _phoneNumber?.ToString() ?? "";
                    var _CreatedAt = DateTime.Parse(_createdAt?.ToString() ?? "");
                    var _OrderItems = _orderItems;

                    var order = new Order
                    {
                        Id = _Id,
                        TotalPrice = _TotalPrice,
                        Percentage = _Percentage,
                        GrandTotal = _GrandTotal,
                        Employee = _Employee,
                        Note = _Note,
                        PhoneNumber = _PhoneNumber,
                        CreatedAt = _CreatedAt,
                        OrderItems = _OrderItems
                    };
                    orders.Add(order);
                }
            }



            return orders;
        }

        public async Task AddOrderAsync(Order order)
        {
            string query = "INSERT INTO orders (total_price, phone_number, created_at, note) " +
                           "VALUES (@TotalPrice, @PhoneNumber, @CreatedAt, @Note) RETURNING id;";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                command.Parameters.AddWithValue("@PhoneNumber", order.PhoneNumber);
                command.Parameters.AddWithValue("@CreatedAt", order.CreatedAt);
                command.Parameters.AddWithValue("@Note", order.Note);

                order.Id = (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task EditOrderAsync(Order order)
        {
            string query = "UPDATE orders SET total_price = @TotalPrice, phone_number = @PhoneNumber, " +
                           "created_at = @CreatedAt, note = @Note WHERE id = @Id;";

            try
            {
                using (var command = new NpgsqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                    command.Parameters.AddWithValue("@PhoneNumber", order.PhoneNumber);
                    command.Parameters.AddWithValue("@CreatedAt", order.CreatedAt);
                    command.Parameters.AddWithValue("@Note", order.Note);
                    command.Parameters.AddWithValue("@Id", order.Id);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteOrderItemAsync(int orderId)
        {
            string query = "DELETE FROM order_details WHERE order_id = @OrderId;";
            try
            {
                using (var command = new NpgsqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            try
            {
                string query = "DELETE FROM orders WHERE id = @OrderId;";
                var _dao = new PostgreSqlDao();
                await _dao.DeleteOrderItemAsync(orderId); // Fix for CS0815: Removed assignment to a variable
                _dao.CloseConnection();
                using (var command = new NpgsqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        private Employee GetEmployee(int employeeId)
        {
            Employee employee = new Employee();
            string query = "SELECT * FROM employee WHERE id = @EmployeeId";
            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@EmployeeId", employeeId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var _id = reader.GetValue(0);
                        var _fullname = reader.GetValue(1);
                        var _username = reader.GetValue(2);
                        var _password = reader.GetValue(3);
                        var _phoneNumber = reader.GetValue(4);
                        var _gender = reader.GetValue(5);
                        var _address = reader.GetValue(6);
                        var _dob = reader.GetValue(7);
                        var _role = reader.GetValue(8);

                        var _Id = Int32.Parse(_id?.ToString() ?? "0");
                        var _Fullname = _fullname?.ToString() ?? "";
                        var _Username = _username?.ToString() ?? "";
                        var _Password = _password?.ToString() ?? "";
                        var _PhoneNumber = _phoneNumber?.ToString() ?? "";
                        var _Gender = _gender?.ToString() ?? "";
                        var _Address = _address?.ToString() ?? "";
                        var _Dob = _dob?.ToString() ?? "";
                        var _Role = _role?.ToString() ?? "";

                        employee = new Employee
                        {
                            ID = _Id.ToString(),
                            Fullname = _Fullname,
                            Username = _Username,
                            Password = _Password,
                            PhoneNumber = _PhoneNumber,
                            Gender = _Gender,
                            Address = _Address,
                            Dob = _Dob,
                            Role = _Role
                        };
                    }
                }
            }

            return employee;
        }

        private List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            var orderItems = new List<OrderItem>();

            string query = "SELECT * FROM order_details WHERE order_id = @OrderId";
            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@OrderId", orderId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var _menu_item_id = reader.GetValue(2);
                        var _quantity = reader.GetValue(3);
                        var _price = reader.GetValue(4);
                        var _dao = new PostgreSqlDao();
                        var _menuItem = _dao.GetMenuItem(Int32.Parse(_menu_item_id?.ToString() ?? "0"));
                        var _category = _dao.GetCategory(_menuItem.CategoryId);
                        _dao.CloseConnection();
                        var _orderMenuItem = new OrderMenuItem
                        {
                            Id = _menuItem.Id,
                            CapitalPrice = _menuItem.CapitalPrice,
                            Category = _category,
                            CategoryId = _menuItem.CategoryId,
                            Discount = _menuItem.Discount,
                            SellPrice = _menuItem.SellPrice,
                            Name = _menuItem.Name,
                            Quantity = _menuItem.Quantity,
                        };

                        var orderItem = new OrderItem
                        {
                            MenuItem = _orderMenuItem,
                            Quantity = int.Parse(_quantity?.ToString() ?? "0"), // Fix for CS0266 and CS8604
                            Price = Decimal.Parse(_price?.ToString() ?? "0") // Fix for CS8604
                        };
                        orderItems.Add(orderItem);
                    }
                }
            }

            return orderItems;
        }

        // Employee Methods for Login

        public bool hasEmployee(string username, string password)
        {
            string query = "SELECT COUNT(*) FROM employee WHERE username = @Username AND password = @Password";
            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                return (long)command.ExecuteScalar() > 0;
            }
        }

        public Employee GetEmployee(string username, string password)
        {
            Employee employee = null;
            string query = "SELECT * FROM employee WHERE username = @Username AND password = @Password";
            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var _id = reader.GetValue(0);
                        var _fullname = reader.GetValue(1);
                        var _username = reader.GetValue(2);
                        var _password = reader.GetValue(3);
                        var _phoneNumber = reader.GetValue(4);
                    }
                }
            }

            return employee;
        }

        public Employee GetEmployeeById(int employeeId)
        {
            Employee employee = new Employee();
            string query = "SELECT * FROM employee WHERE id = @EmployeeId";
            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@EmployeeId", employeeId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var _id = reader.GetValue(0);
                        var _fullname = reader.GetValue(1);
                        var _username = reader.GetValue(2);
                        var _password = reader.GetValue(3);
                        var _phoneNumber = reader.GetValue(4);
                        var _gender = reader.GetValue(5);
                        var _address = reader.GetValue(6);
                        var _dob = reader.GetValue(7);
                        var _role = reader.GetValue(8);

                        var _Id = Int32.Parse(_id?.ToString() ?? "0");
                        var _Fullname = _fullname?.ToString() ?? "";
                        var _Username = _username?.ToString() ?? "";
                        var _Password = _password?.ToString() ?? "";
                        var _PhoneNumber = _phoneNumber?.ToString() ?? "";
                        var _Gender = _gender?.ToString() ?? "";
                        var _Address = _address?.ToString() ?? "";
                        var _Dob = _dob?.ToString() ?? "";
                        var _Role = _role?.ToString() ?? "";

                        employee = new Employee
                        {
                            ID = _Id.ToString(),
                            Fullname = _Fullname,
                            Username = _Username,
                            Password = _Password,
                            PhoneNumber = _PhoneNumber,
                            Gender = _Gender,
                            Address = _Address,
                            Dob = _Dob,
                            Role = _Role
                        };
                    }
                }
            }

            return employee;
        }

        public bool setRegisterShift(string username, DateTime day, int shift)
        {
            // Kiểm tra xem bản ghi đã tồn tại
            string checkQuery = "SELECT COUNT(*) FROM shifts WHERE _day::date = @Day::date AND shift = @Shift;";
            using (var command = new NpgsqlCommand(checkQuery, _connection))
            {
                command.Parameters.AddWithValue("@Day", day.Date); // Chỉ lấy phần ngày, bỏ qua thời gian
                command.Parameters.AddWithValue("@Shift", shift);
                var count = (long)command.ExecuteScalar();
                if (count == 0)
                {
                    // Nếu chưa tồn tại, tạo mới bản ghi
                    username += ',';
                    string insertQuery = "INSERT INTO shifts (_day, shift, employees_username) VALUES (@Day::date, @Shift, @Username);";
                    using (var insertCommand = new NpgsqlCommand(insertQuery, _connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Day", day.Date);
                        insertCommand.Parameters.AddWithValue("@Shift", shift);
                        insertCommand.Parameters.Add(new NpgsqlParameter("@Username", NpgsqlDbType.Varchar) { Value = username });
                        insertCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Nếu đã tồn tại, cập nhật cột username
                    string updateQuery = "UPDATE shifts SET employees_username = employees_username || @Username || ',' WHERE _day::date = @Day::date AND shift = @Shift AND employees_username NOT LIKE @Pattern;"; using (var updateCommand = new NpgsqlCommand(updateQuery, _connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Day", day.Date);
                        updateCommand.Parameters.AddWithValue("@Shift", shift);
                        updateCommand.Parameters.AddWithValue("@Pattern", $"%{username}%");
                        updateCommand.Parameters.Add(new NpgsqlParameter("@Username", NpgsqlDbType.Varchar) { Value = username });
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }
        public List<int> getRegisterShift(string username, DateTime day)
        {
            List<int> results = new List<int>();

            string query = @"
            SELECT (_day::date - @StartDay::date + 1) + shift * 8 as shift_index 
            FROM shifts 
            WHERE @StartDay::date <= _day::date 
            AND _day::date <= (@StartDay::date + interval '6 days')
            AND employees_username LIKE @Pattern";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                // Sử dụng day.Date để chỉ lấy phần ngày, bỏ phần thời gian
                command.Parameters.AddWithValue("@StartDay", day.Date);
                // Tìm username trong chuỗi employees_username
                command.Parameters.AddWithValue("@Pattern", $"%{username}%");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(reader.GetInt32(0));
                    }
                }
            }

            return results;
        }
        public List<int> getFullRegisteredShift(DateTime day)
        {
            List<int> results = new List<int>();

            string query = @"
            SELECT (_day::date - @StartDay::date + 1) + shift * 8 as shift_index 
            FROM shifts 
            WHERE @StartDay::date <= _day::date 
            AND _day::date <= (@StartDay::date + interval '6 days')
            AND LENGTH(employees_username) - LENGTH(REPLACE(employees_username, ',', '')) >= 5";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@StartDay", day.Date);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(reader.GetInt32(0));
                    }
                }
            }

            return results;
        }



        public void deleteRegisterShift(DateTime day, int shift)
        {
            string updateQuery = @"
            UPDATE shifts
            SET employees_username = REPLACE(employees_username, @UsernameToRemove, '')
            WHERE _day::date = @Day::date AND shift = @Shift";

            using (var command = new NpgsqlCommand(updateQuery, _connection))
            {
                command.Parameters.AddWithValue("@Day", day.Date);
                command.Parameters.AddWithValue("@Shift", shift);
                command.Parameters.AddWithValue("@UsernameToRemove", CurrentUser.username + ",");
                command.ExecuteNonQuery();
            }
        }


        public string getFullRegisteredShiftForManager(DateTime day, int shift)
        {

            string result = "";
            string query = @"
            SELECT employees_username 
            FROM shifts 
            WHERE _day::date = @StartDay::date 
            AND shift = @Shift";

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@StartDay", day.Date);
                command.Parameters.AddWithValue("@Shift", shift);

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader.GetString(0);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return result;
        }

        public bool checkin()
        {
            DateTime today = DateTime.Now;
            string insertQuery = "INSERT INTO ShiftCheck (_day, employees_username, checkin) VALUES (@Day::date, @Username, @CheckinTime);";
            using (var insertCommand = new NpgsqlCommand(insertQuery, _connection))
            {
                insertCommand.Parameters.AddWithValue("@Day", today.Date);
                insertCommand.Parameters.AddWithValue("@Username", CurrentUser.username);
                insertCommand.Parameters.AddWithValue("@CheckinTime", today.TimeOfDay);

                insertCommand.ExecuteNonQuery();
            }

            return true;
        }
        public bool checkout()
        {
            DateTime today = DateTime.Now;
            string updateQuery = @"
            UPDATE ShiftCheck 
            SET checkout = @Checkout, status = @Status 
            WHERE employees_username = @Username 
            AND checkout IS NULL;";

            // Lấy thời gian checkin từ bảng
            TimeSpan checkinTime;
            string selectQuery = "SELECT checkin FROM ShiftCheck WHERE _day::date = @Day::date AND employees_username = @Username AND checkout IS NULL;";

            using (var selectCommand = new NpgsqlCommand(selectQuery, _connection))
            {
                selectCommand.Parameters.AddWithValue("@Day", today.Date);
                selectCommand.Parameters.AddWithValue("@Username", CurrentUser.username);

                var result = selectCommand.ExecuteScalar();
                if (result != null)
                {
                    checkinTime = (TimeSpan)result;
                }
                else
                {
                    return false;
                }
            }

            string status = SHIFT.GetCheckinStatus(checkinTime, today.TimeOfDay);

            using (var updateCommand = new NpgsqlCommand(updateQuery, _connection))
            {
                // Thêm tham số cho truy vấn
                updateCommand.Parameters.AddWithValue("@Day", today.Date);
                updateCommand.Parameters.AddWithValue("@Username", CurrentUser.username);
                updateCommand.Parameters.AddWithValue("@Checkout", today.TimeOfDay);
                updateCommand.Parameters.AddWithValue("@Status", status);

                int rowsAffected = updateCommand.ExecuteNonQuery();

                return rowsAffected > 0; // Trả về true nếu có dòng được cập nhật
            }
        }


        public bool getShiftstatistics(List<ShiftStatistic> shiftStatistics)
        {
            string query = @"
               SELECT employees_username,
                      COUNT(*) AS total,
                      SUM(CASE WHEN status = 'LATE' THEN 1 ELSE 0 END) AS late,
                      SUM(CASE WHEN status = 'ON_TIME' THEN 1 ELSE 0 END) AS on_time
               FROM ShiftCheck
               GROUP BY employees_username;";

            try
            {
                using (var command = new NpgsqlCommand(query, _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        shiftStatistics.Clear(); // Clear the list before adding new data

                        while (reader.Read())
                        {
                            var statistic = new ShiftStatistic
                            {
                                employees_username = reader.GetString(reader.GetOrdinal("employees_username")),
                                total = reader.GetInt32(reader.GetOrdinal("total")),
                                late = reader.GetInt32(reader.GetOrdinal("late")),
                                on_time = reader.GetInt32(reader.GetOrdinal("on_time"))
                            };

                            shiftStatistics.Add(statistic);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return shiftStatistics.Count > 0; // Return true if there is data
        }
    }
}
