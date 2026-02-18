using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Bson;
using POS.Models;
using System.Collections.ObjectModel;

namespace POS.Services.Dao;

public interface IDao
{
    List<Category> GetCategories();
    Category? GetCategory(int id);
    List<MenuItem> GetMenuItems();
    MenuItem GetMenuItem(int id);
    List<OrderItem> GetOrderItems();
    public bool DeleteEmployee(string id);
    public bool AddEmployee(Employee newEmployee);
    public bool UpdateEmployee(int id, Employee newEmployee);



    /**/
    public float getTotalSales();
    public float AverageRevenuePerTransaction();
    public float CustomerReturning();
    public KeyValuePair<string, float> getTopSelling();


    public List<float> getTotalSaleByWeek(DateTime dateTime);
    public List<float> getTotalSaleInMonth();
    public List<float> getTotalSaleByYear();



    /*Column chatrts */
    public List<KeyValuePair<string, float>> getTopSellingByWeek();
    public List<KeyValuePair<string, float>> getTopSellingByMonth();
    public List<KeyValuePair<string, float>> getTopSellingByYear();

    List<ComboMenuItem> GetComboMenuItems();
    List<ComboItem> GetComboItemsByComboMenuItemsId(int id);
    List<ComboItem> GetComboItems();
    List<Discount> GetDiscounts();
    Discount? GetDiscount(int menuItemId);
    ObservableCollection<POS.Models.Employee> GetEmployees();
    Task AddCategoryAsync(Category category);
    Task AddMenuItemAsync(MenuItem menuItem);
    Task AddComboMenuItemAsync(ComboMenuItem comboMenuItem);
    Task AddComboItemAsync(ComboItem comboItem);
    Task AddDiscountAsync(Discount discount);
    Task AddMenuItemDiscountAsync(int discountId, int menuItemId);

    // Update Methods
    Task EditMenuItemAsync(MenuItem menuItem);
    Task EditComboMenuItemAsync(ComboMenuItem comboMenuItem);
    Task EditDiscountAsync(Discount discount);
    public List<KeyValuePair<float, float>> getCompareCapitalWithValueByMonth();
    public List<KeyValuePair<float, float>> getCompareCapitalWithValueByYear();
    /*customer*/

    // Delete Methods
    Task DeleteCategoryAsync(int categoryId);
    Task DeleteMenuItemAsync(int menuItemId);
    Task DeleteComboMenuItemAsync(int comboMenuItemId);
    Task DeleteComboItemAsync(int comboMenuItemId, int menuItemId);
    Task DeleteComboItemAsync(int comboMenuItemId);
    Task DeleteDiscountAsync(int discountId);
    public bool DeleteCustomer(string telephoneNumber);
    public bool AddCustomer(Customer customer);
    public bool UpdateCustomer(Customer customer);
    public bool CustomerAddPoint(string telephoneNumber, float point);
    Task DeleteMenuItemDiscountAsync(int menuItemId);

    // Order Methods
    List<Item> GetComboItems(int comboId);

    // History Order
    public List<Order> GetOrders();
    public Task AddOrderAsync(Order order);
    public Task EditOrderAsync(Order order);
    public Task DeleteOrderAsync(int orderId);

    // Employee Methods for Login
    bool hasEmployee(string username, string password);
    Employee GetEmployeeById(int employeeId);
    Employee GetEmployee(string username, string password);



    /*For register shift */

    public bool setRegisterShift(string username, DateTime day, int shift);
    public List<int> getRegisterShift(string username, DateTime day);
    public List<int> getFullRegisteredShift(DateTime day);
    public void deleteRegisterShift(DateTime day, int shift);
    public string getFullRegisteredShiftForManager(DateTime day, int shift);
    public bool checkin();
    public bool checkout();
    public bool getShiftstatistics(List<ShiftStatistic> shiftStatistics);


}