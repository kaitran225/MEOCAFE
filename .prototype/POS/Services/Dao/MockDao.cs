//using POS.Models;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;

//namespace POS.Services.Dao
//{
//    public class MockDao : IDao
//    {

//        private ObservableCollection<Employee> employees = new ObservableCollection<Employee>
//            {
//                new Employee()
//                    {
//                        Fullname = "Jose Murphy",
//                        Username =  "josemurphy1",
//                        Password = "1111",
//                        ID = "E001",
//                        PhoneNumber = "1234567890",
//                        Gender = "Male",
//                        Address = "123 Elm St",
//                        Dob = "1990-05-10",
//                        Role = "Manager",
//                        //AvatarPath = "MockImage/employee1.jpg",
//                    },
//                new Employee()
//                {
//                    Fullname = "Alice Johnson",
//                    Username =   "alicej",
//                    Password = "1234",
//                    ID = "E002",
//                    PhoneNumber = "0987654321",
//                    Gender = "Female",
//                    Address = "456 Oak St",
//                    Dob = "1985-11-23",
//                    Role = "Staff",
//                  //  AvatarPath = "MockImage/employee1.jpg",
//                },
//                new Employee()
//                {
//                    Fullname = "Bob Smith",
//                    Username =  "bobsmith",
//                    Password = "abcd",
//                    ID = "E003",
//                    PhoneNumber = "1112223333",
//                    Gender = "Male",
//                    Address = "789 Pine St",
//                    Dob = "1992-02-14",
//                    Role = "Staff",
//                   // AvatarPath = "MockImage/employee1.jpg"

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E004",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E005",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E006",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E007",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E008",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E009",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E0010",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E0011",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E0012",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E0013",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E0014",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E0015",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E0016",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },   new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E0017",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E0018",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },   new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E0019",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "Carol White",
//                    Username =   "carolwhite",
//                    Password =  "qwerty",
//                    ID = "E0020",
//                    PhoneNumber = "2223334444",
//                    Gender = "Female",
//                    Address = "321 Maple St",
//                    Dob = "1988-07-30",
//                    Role = "Supervisor",
//                  //  AvatarPath = "MockImage/employee1.jpg",

//                },
//                new Employee()
//                {
//                    Fullname = "David Brown",
//                    Username =  "davidb",
//                    Password ="xyz123",
//                    ID =  "E0021",
//                    PhoneNumber = "3334445555",
//                    Gender = "Male",
//                    Address = "654 Birch St",
//                    Dob = "1995-12-01",
//                    Role = "Staff",
//                 //   AvatarPath = "MockImage/employee1.jpg",
//                }

//            };

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
//            return employees;
//        }
//        public bool DeleteEmployee(string id)
//        {
//            var employeeToDelete = employees.FirstOrDefault(e => e.ID == id);

//            if (employeeToDelete == null)
//            {
//                return false;
//            }

//            employees.Remove(employeeToDelete);

//            return true;
//        }
//        public bool AddEmployee(Employee newEmployee)
//        {

//            employees.Add(newEmployee);
//            return true;
//        }
//        public bool UpdateEmployee(int id, Employee newEmployee)
//        {

//            return true;
//        }

//        /*For stactistics */
//        public List<float> getTotalSaleByWeek(DateTime dateTime)
//        {
//            Random random = new Random();
//            List<float> sales = new List<float>();

//            for (int i = 0; i < 7; i++)
//            {
//                float sale = (float)(random.Next(1000000, 10000001));
//                sales.Add(sale);
//            }

//            return sales;
//        }
//        public List<float> getTotalSaleInMonth()
//        {
//            Random random = new Random();
//            List<float> sales = new List<float>();

//            for (int i = 0; i < 12; i++)
//            {
//                float sale = (float)(random.Next(1000000, 10000001));
//                sales.Add(sale);
//            }

//            return sales;
//        }
//        public List<float> getTotalSaleByYear()
//        {
//            Random random = new Random();
//            List<float> sales = new List<float>();

//            for (int i = 0; i < 5; i++)
//            {
//                float sale = (float)(random.Next(30000000, 300000000));
//                sales.Add(sale);
//            }

//            return sales;
//        }
//        public List<KeyValuePair<string, float>> getTopSellingByWeek()
//        {
//            Random random = new Random();
//            List<KeyValuePair<string, float>> topSelling = new List<KeyValuePair<string, float>>();

//            // Danh sách các loại đồ ăn và đồ uống
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


//        /*For customer */

//        public bool DeleteCustomer(string telephoneNumber)
//        {

//            return true;
//        }
//        public bool AddCustomer(Customer customer)
//        {

//            return true;
//        }
//        public bool UpdateCustomer(Customer customer)
//        {
//            return true;
//        }
//        public bool CustomerAddPoint(string telephoneNumber, float point)
//        {
//            return
//                true;
//        }


//        public float getTotalSales()
//        {
//            return 0;
//        }
//        public float AverageRevenuePerTransaction()
//        {
//            return 0;
//        }
//        public float CustomerReturning()
//        {
//            return 0;
//        }
//        public KeyValuePair<string, float> getTopSelling()
//        {
//            return new KeyValuePair<string, float>("first", 0);
//        }
//    }
//}