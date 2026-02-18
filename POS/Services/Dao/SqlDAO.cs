//using Npgsql;
//using POS.Models;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;

//namespace POS.Services.Dao
//{
//    public class SqlDAO : IDao
//    {
//        string connectionString = "Host=localhost;Port=5792;Username=postgres;Password=postgres;Database=db;";
//        private ObservableCollection<Employee> employees;

//        private ObservableCollection<Customer> customers = new ObservableCollection<Customer>
//            {
//                new ()
//                    {
//                        Fullname = "Jose Murphy" ,
//                        PhoneNumber = "1234567890",
//                        Point = ""
//                    },

//        };
//        public List<Category> GetCategories()
//        {
//            var categories = new List<Category>
//            {
//                new()
//                {
//                    Id = 1,
//                    Name = "Café",
//                    MenuItems = new List<MenuItem>
//                    {
//                        new() { Id = 1, Name = "Americano", SellPrice = 100000, CapitalPrice = 1000, ImagePath = "MockImage/americano" },
//                        new() { Id = 2, Name = "Cappuccino", SellPrice = 80000, CapitalPrice = 1000, ImagePath = "MockImage/cappuccino" },
//                        new() { Id = 3, Name = "Latte", SellPrice = 120000, CapitalPrice = 1000, ImagePath = "MockImage/latte" },
//                        new() { Id = 4, Name = "Espresso", SellPrice = 150000, CapitalPrice = 1000, ImagePath = "MockImage/espresso" },
//                        new() { Id = 5, Name = "Glace", SellPrice = 150000, CapitalPrice = 1000, ImagePath = "MockImage/glace" },
//                        new() { Id = 6, Name = "Mocha", SellPrice = 150000, CapitalPrice = 1000, ImagePath = "MockImage/mocha" },
//                        new() { Id = 7, Name = "Americano", SellPrice = 100000, CapitalPrice = 1000, ImagePath = "MockImage/americano" },
//                        new() { Id = 8, Name = "Cappuccino", SellPrice = 80000, CapitalPrice = 1000, ImagePath = "MockImage/cappuccino" },
//                        new() { Id = 9, Name = "Latte", SellPrice = 120000, CapitalPrice = 1000, ImagePath = "MockImage/latte" },
//                        new() { Id = 10, Name = "Espresso", SellPrice = 150000, CapitalPrice = 1000, ImagePath = "MockImage/espresso" },
//                        new() { Id = 11, Name = "Glace", SellPrice = 150000, CapitalPrice = 1000, ImagePath = "MockImage/glace" },
//                        new() { Id = 12, Name = "Mocha", SellPrice = 150000, CapitalPrice = 1000, ImagePath = "MockImage/mocha" },
//                    }
//                },
//                new()
//                {
//                    Id = 2,
//                    Name = "Cake",
//                    MenuItems = new List<MenuItem>
//                    {
//                        new() { Id = 13, Name = "Chocolate Cake", SellPrice = 34000, CapitalPrice = 1000, ImagePath = "MockImage/chocolate_cake" },
//                        new() { Id = 14, Name = "Flan", SellPrice = 56000, CapitalPrice = 1000, ImagePath = "MockImage/flan" }
//                    }
//                }
//            };

//            return categories;
//        }
//        public List<MenuItem> GetMenuItems()
//        {
//            var categories = GetCategories();
//            var menuItems = new List<MenuItem>();

//            foreach (var category in categories)
//            {
//                menuItems.AddRange(category.MenuItems);
//            }

//            return menuItems;
//        }
//        public List<OrderItem> GetOrderItems()
//        {
//            List<Category> categories = GetCategories();

//            Category cafe = categories[0];
//            Category cake = categories[1];

//            List<MenuItem> menuItemsOfCafe = cafe.MenuItems;
//            List<MenuItem> menuItemsOfCake = cake.MenuItems;

//            return new List<OrderItem>
//            {
//                new OrderItem { MenuItem = menuItemsOfCafe[0], Quantity = 1 },
//                new OrderItem { MenuItem = menuItemsOfCafe[4], Quantity = 2 },
//                new OrderItem { MenuItem = menuItemsOfCake[0], Quantity = 3 },
//            };
//        }



//        /*Employee*/

//        public ObservableCollection<Employee> GetEmployees()
//        {
//            ObservableCollection<Employee> employees = new ObservableCollection<Employee>();

//            using (var connection = new NpgsqlConnection(connectionString))
//            {
//                try
//                {
//                    connection.Open();

//                    using (var cmd = new NpgsqlCommand("SELECT id, fullname, username, password, phone_number, gender, address, dob, role FROM employee", connection))
//                    {
//                        using (var reader = cmd.ExecuteReader())
//                        {
//                            while (reader.Read())
//                            {
//                                Employee employee = new Employee
//                                {
//                                    ID = reader["id"].ToString(),
//                                    Fullname = reader["fullname"].ToString(),
//                                    Username = reader["username"].ToString(),
//                                    Password = reader["password"].ToString(),
//                                    PhoneNumber = reader["phone_number"].ToString(),
//                                    Gender = reader["gender"].ToString(),
//                                    Address = reader["address"].ToString(),
//                                    Dob = reader["dob"].ToString(),
//                                    Role = reader["role"].ToString()
//                                };

//                                employees.Add(employee);
//                            }
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {

//                }
//            }

//            return employees;
//        }
//        public bool AddEmployee(Employee newEmployee)
//        {

//            string query = @"INSERT INTO employee (fullname, username, password, phone_number, gender, address, dob, role)
//                     VALUES ( @Fullname, @Username, @Password, @PhoneNumber, @Gender, @Address, @Dob, @Role)";

//            try
//            {
//                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
//                {
//                    connection.Open();

//                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
//                    {
//                        command.Parameters.AddWithValue("@Fullname", newEmployee.Fullname);
//                        command.Parameters.AddWithValue("@Username", newEmployee.Username);
//                        command.Parameters.AddWithValue("@Password", newEmployee.Password);
//                        command.Parameters.AddWithValue("@PhoneNumber", newEmployee.PhoneNumber);
//                        command.Parameters.AddWithValue("@Gender", newEmployee.Gender);
//                        command.Parameters.AddWithValue("@Address", newEmployee.Address);
//                        command.Parameters.AddWithValue("@Dob", DateTime.Parse(newEmployee.Dob)); // Đảm bảo định dạng ngày tháng đúng
//                        command.Parameters.AddWithValue("@Role", newEmployee.Role);

//                        int rowsAffected = command.ExecuteNonQuery();

//                        if (rowsAffected > 0)
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error: {ex.Message}");
//            }

//            return false;
//        }
//        public bool DeleteEmployee(string id)
//        {

//            string query = @"DELETE FROM employee
//                    WHERE ID = @Id ";

//            try
//            {
//                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
//                {
//                    connection.Open();

//                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
//                    {
//                        command.Parameters.AddWithValue("@Id", int.Parse(id));


//                        int rowsAffected = command.ExecuteNonQuery();

//                        if (rowsAffected > 0)
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error: {ex.Message}");
//            }

//            return false;
//        }



//        public bool UpdateEmployee(int id, Employee newEmployee)
//        {
//            string query = @"UPDATE employee
//                            SET fullname = @fullname,
//                             username = @username,
//                             phone_number = @phone_number,
//                             gender = @gender,
//                             address = @address,
//                             dob = @dob,
//                                role = @role
//                                WHERE id = @id";

//            try
//            {
//                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
//                {
//                    connection.Open();

//                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
//                    {
//                        command.Parameters.AddWithValue("@Id", id);
//                        command.Parameters.AddWithValue("@fullname", newEmployee.Fullname);
//                        command.Parameters.AddWithValue("@username", newEmployee.Username);
//                        command.Parameters.AddWithValue("@phone_number", newEmployee.PhoneNumber);
//                        command.Parameters.AddWithValue("@gender", newEmployee.Gender);
//                        command.Parameters.AddWithValue("@address", newEmployee.Address);
//                        command.Parameters.AddWithValue("@dob", DateTime.Parse(newEmployee.Dob));
//                        command.Parameters.AddWithValue("@role", newEmployee.Role);

//                        int rowsAffected = command.ExecuteNonQuery();

//                        if (rowsAffected > 0)
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error: {ex.Message}");
//            }

//            return false;
//        }

//        /*For stactistics */
//        /*equivalence with final_price trong Orders */


//        public List<float> getTotalSaleByWeek(DateTime referenceDate)
//        {
//            var weeklySales = new List<float>();

//            using (var connection = new NpgsqlConnection(connectionString))
//            {
//                connection.Open();

//                var query = @"
//        WITH week_days AS (
//            SELECT
//                generate_series(
//                    -- Xác định ngày bắt đầu tuần
//                    CASE
//                        WHEN EXTRACT(DOW FROM @reference_date) = 1 THEN @reference_date::DATE - INTERVAL '6 days'
//                        ELSE @reference_date::DATE - INTERVAL '1 day' * (EXTRACT(DOW FROM @reference_date) - 1)
//                    END,
//                    -- Ngày kết thúc tuần
//                    CASE
//                        WHEN EXTRACT(DOW FROM @reference_date) = 1 THEN @reference_date::DATE
//                        ELSE @reference_date::DATE + INTERVAL '1 day' * (7 - EXTRACT(DOW FROM @reference_date))
//                    END,
//                    '1 day'
//                ) AS sale_date
//        )
//        SELECT
//            wd.sale_date,
//            COALESCE(SUM(o.grand_total), 0) AS daily_sales
//        FROM
//            week_days wd
//        LEFT JOIN Orders o ON DATE(o.created_at) = wd.sale_date
//        GROUP BY wd.sale_date
//        ORDER BY wd.sale_date;";

//                using (var command = new NpgsqlCommand(query, connection))
//                {
//                    // Thêm tham số `@reference_date` vào truy vấn
//                    command.Parameters.AddWithValue("@reference_date", referenceDate);

//                    using (var reader = command.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            float dailySales = Convert.ToSingle(reader.GetDouble(1));
//                            weeklySales.Add(dailySales);
//                        }
//                    }
//                }
//            }

//            return weeklySales;
//        }
//        public List<float> getTotalSaleInMonth()
//        {
//            var monthlySales = new List<float>();

//            using (var connection = new NpgsqlConnection(connectionString))
//            {
//                connection.Open();

//                var query = @"
//        SELECT
//            EXTRACT(MONTH FROM o.created_at) AS month,
//            COALESCE(SUM(o.grand_total), 0) AS total_sales
//        FROM
//            Orders o
//        WHERE
//            EXTRACT(YEAR FROM o.created_at) = EXTRACT(YEAR FROM CURRENT_DATE) -- Lọc theo năm hiện tại
//        GROUP BY
//            EXTRACT(MONTH FROM o.created_at)
//        ORDER BY
//            month;";

//                using (var command = new NpgsqlCommand(query, connection))
//                {
//                    using (var reader = command.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            // Đọc tổng doanh thu cho mỗi tháng
//                            float monthlySale = Convert.ToSingle(reader.GetDouble(1));
//                            monthlySales.Add(monthlySale);
//                        }
//                    }
//                }
//            }

//            return monthlySales;
//        }
//        public List<float> getTotalSaleByYear()
//        {
//            var yearlySales = new List<float>();

//            using (var connection = new NpgsqlConnection(connectionString))
//            {
//                connection.Open();

//                var query = @"
//                    SELECT
//                        EXTRACT(YEAR FROM o.created_at) AS year,
//                        COALESCE(SUM(o.grand_total), 0) AS total_sales
//                    FROM
//                        Orders o
//                    GROUP BY
//                        EXTRACT(YEAR FROM o.created_at)
//                    ORDER BY
//                        year;";

//                using (var command = new NpgsqlCommand(query, connection))
//                {
//                    using (var reader = command.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            // Đọc tổng doanh thu cho mỗi năm
//                            float yearlySale = Convert.ToSingle(reader.GetDouble(1));
//                            yearlySales.Add(yearlySale);
//                        }
//                    }
//                }
//            }

//            return yearlySales;
//        }


//        public List<KeyValuePair<string, float>> getTopSellingByWeek()
//        {
//            Random random = new Random();
//            List<KeyValuePair<string, float>> topSelling = new List<KeyValuePair<string, float>>();

//            string[] items = new string[]
//            {
//            "Nước cam",
//            "Cappuccino",
//            "Cà phê",
//            "Trà sữa",
//            "Bánh ngọt",
//            "Sinh tố",
//            "Latte",
//            "Espresso",
//            "Nước dừa",
//            "Bánh mì"
//            };

//            foreach (var item in items)
//            {
//                float sale = (float)(random.Next(300000, 1000001)); // Tạo số ngẫu nhiên trong khoảng từ 300,000 đến 1,000,000
//                topSelling.Add(new KeyValuePair<string, float>(item, sale));
//            }

//            return topSelling;
//        }
//        public List<KeyValuePair<string, float>> getTopSellingByMonth()
//        {
//            Random random = new Random();
//            List<KeyValuePair<string, float>> topSelling = new List<KeyValuePair<string, float>>();

//            string[] items = new string[]
//            {
//            "Nước cam",
//            "Cappuccino",
//            "Cà phê",
//            "Trà sữa",
//            "Bánh ngọt",
//            "Sinh tố",
//            "Latte",
//            "Espresso",
//            "Nước dừa",
//            "Bánh mì"
//            };

//            foreach (var item in items)
//            {
//                float sale = (float)(random.Next(9000000, 30000000));
//                topSelling.Add(new KeyValuePair<string, float>(item, sale));
//            }

//            return topSelling;
//        }
//        public List<KeyValuePair<string, float>> getTopSellingByYear()
//        {
//            Random random = new Random();
//            List<KeyValuePair<string, float>> topSelling = new List<KeyValuePair<string, float>>();

//            string[] items = new string[]
//            {
//            "Nước cam",
//            "Cappuccino",
//            "Cà phê",
//            "Trà sữa",
//            "Bánh ngọt",
//            "Sinh tố",
//            "Latte",
//            "Espresso",
//            "Nước dừa",
//            "Bánh mì"
//            };

//            foreach (var item in items)
//            {
//                float sale = (float)(random.Next(30000000, 100000001));
//                topSelling.Add(new KeyValuePair<string, float>(item, sale));
//            }

//            return topSelling;
//        }
//        /*For customer */

//        public bool DeleteCustomer(string telephoneNumber)
//        {
//            try
//            {
//                using (var connection = new NpgsqlConnection(connectionString))
//                {
//                    connection.Open();
//                    var query = "DELETE FROM customer WHERE phone_number = @phone_number";
//                    using (var command = new NpgsqlCommand(query, connection))
//                    {
//                        command.Parameters.AddWithValue("@phone_number", telephoneNumber);
//                        int rowsAffected = command.ExecuteNonQuery();
//                        return rowsAffected > 0;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                // Log exception (optional)
//                return false;
//            }
//        }
//        public bool AddCustomer(Customer customer)
//        {
//            try
//            {
//                using (var connection = new NpgsqlConnection(connectionString))
//                {
//                    connection.Open();
//                    var query = "INSERT INTO customer (name, phone_number, point) VALUES (@name, @phone_number, @point)";
//                    using (var command = new NpgsqlCommand(query, connection))
//                    {
//                        command.Parameters.AddWithValue("@name", customer.Fullname);
//                        command.Parameters.AddWithValue("@phone_number", customer.PhoneNumber);
//                        command.Parameters.AddWithValue("@point", customer.Point);
//                        int rowsAffected = command.ExecuteNonQuery();
//                        return rowsAffected > 0;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//        }
//        public bool UpdateCustomer(Customer customer)
//        {
//            try
//            {
//                using (var connection = new NpgsqlConnection(connectionString))
//                {
//                    connection.Open();
//                    var query = "UPDATE customer SET name = @name, point = @point WHERE phone_number = @phone_number";
//                    using (var command = new NpgsqlCommand(query, connection))
//                    {
//                        command.Parameters.AddWithValue("@name", customer.Fullname);
//                        command.Parameters.AddWithValue("@phone_number", customer.PhoneNumber);
//                        command.Parameters.AddWithValue("@point", customer.Point);
//                        int rowsAffected = command.ExecuteNonQuery();
//                        return rowsAffected > 0;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                // Log exception (optional)
//                return false;
//            }
//        }
//        public bool CustomerAddPoint(string telephoneNumber, float point)
//        {
//            try
//            {
//                using (var connection = new NpgsqlConnection(connectionString))
//                {
//                    connection.Open();
//                    var query = "UPDATE customer SET point = point + @point WHERE phone_number = @phone_number";
//                    using (var command = new NpgsqlCommand(query, connection))
//                    {
//                        command.Parameters.AddWithValue("@phone_number", telephoneNumber);
//                        command.Parameters.AddWithValue("@point", point);
//                        int rowsAffected = command.ExecuteNonQuery();
//                        return rowsAffected > 0;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//        }


//        public List<KeyValuePair<float, float>> getCompareCapitalWithValueByMonth()
//        {
//            List<KeyValuePair<float, float>> a = new List<KeyValuePair<float, float>>();
//            return a;
//        }
//        public List<KeyValuePair<float, float>> getCompareCapitalWithValueByYear()
//        {
//            List<KeyValuePair<float, float>> a = new List<KeyValuePair<float, float>>();
//            return a;
//        }


//        public float getTotalSales()
//        {
//            float totalSales = 0;

//            using (var connection = new NpgsqlConnection(connectionString))
//            {
//                connection.Open();

//                var query = @"
//        SELECT COALESCE(SUM(grand_total), 0) AS total_sales
//        FROM Orders
//        WHERE EXTRACT(YEAR FROM created_at) = EXTRACT(YEAR FROM NOW());
//        ";

//                using (var command = new NpgsqlCommand(query, connection))
//                {
//                    using (var reader = command.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            totalSales = Convert.ToSingle(reader["total_sales"]);
//                        }
//                    }
//                }
//            }

//            return totalSales;
//        }

//        public float AverageRevenuePerTransaction()
//        {
//            float averageRevenue = 0;

//            using (var connection = new NpgsqlConnection(connectionString))
//            {
//                connection.Open();

//                var query = @"
//            SELECT COALESCE(AVG(grand_total), 0) AS average_revenue
//            FROM Orders;
//            ";

//                using (var command = new NpgsqlCommand(query, connection))
//                {
//                    using (var reader = command.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            averageRevenue = Convert.ToSingle(reader["average_revenue"]);
//                        }
//                    }
//                }
//            }

//            return averageRevenue;
//        }

//        public float CustomerReturning()
//        {
//            float returningCustomerRatio = 0;

//            using (var connection = new NpgsqlConnection(connectionString))
//            {
//                connection.Open();

//                var query = @"
//        SELECT 
//            COALESCE(
//                CAST(COUNT(DISTINCT phone_number) FILTER (WHERE order_count > 1) AS FLOAT) /
//                NULLIF(COUNT(DISTINCT phone_number), 0),
//            0) AS returning_customer_ratio
//        FROM (
//            SELECT phone_number, COUNT(*) AS order_count
//            FROM Orders
//            WHERE phone_number IS NOT NULL
//            GROUP BY phone_number
//        ) subquery;
//        ";

//                using (var command = new NpgsqlCommand(query, connection))
//                {
//                    using (var reader = command.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            returningCustomerRatio = Convert.ToSingle(reader["returning_customer_ratio"]);
//                        }
//                    }
//                }
//            }

//            return returningCustomerRatio;
//        }

//        public KeyValuePair<string, float> getTopSelling()
//        {
//            string productName = null;
//            float totalSales = 0;

//            using (var connection = new NpgsqlConnection(connectionString))
//            {
//                connection.Open();

//                var query = @"
//                    SELECT 
//                        mi.name AS menu_item_name, 
//                        SUM(od.price) AS total_sales
//                    FROM 
//                        order_details od
//                    JOIN 
//                        Orders o ON od.order_id = o.id
//                    JOIN 
//                        menu_items mi ON od.menu_item_id = mi.id
//                    WHERE 
//                        DATE_PART('year', o.created_at) = DATE_PART('year', CURRENT_DATE)
//                    GROUP BY 
//                        mi.name
//                    ORDER BY 
//                        total_sales DESC
//                    LIMIT 1;
//                ";

//                using (var command = new NpgsqlCommand(query, connection))
//                {
//                    using (var reader = command.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            productName = reader["menu_item_name"].ToString();
//                            totalSales = Convert.ToSingle(reader["total_sales"]);
//                        }
//                    }
//                }
//            }

//            return new KeyValuePair<string, float>(productName, totalSales);
//        }
//    }
//}