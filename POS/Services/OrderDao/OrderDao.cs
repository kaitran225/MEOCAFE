using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using POS.Services.Dao;

namespace POS.Services.OrderDao
{
    public class OrderDao : IOrderDao
    {
        private readonly DatabaseManager db;

        public OrderDao()
        {
            db = new DatabaseManager();
        }

        public async Task AddOrder(Models.Order order)
        {
            db.Connect();

            try
            {
                db.Command.CommandText = "INSERT INTO orders (id, total_price, grand_total, employee_id, phone_number, note) VALUES (@id, @total_price, @grand_total, @employee_id, @phone_number, @note)";
                db.Command.Parameters.AddWithValue("id", order.Id);
                db.Command.Parameters.AddWithValue("total_price", order.TotalPrice);
                db.Command.Parameters.AddWithValue("employee_id", Int32.Parse(order.Employee.ID.ToString()));
                db.Command.Parameters.AddWithValue("note", order.Note);
                db.Command.Parameters.AddWithValue("phone_number", order.PhoneNumber);
                db.Command.Parameters.AddWithValue("grand_total", order.GrandTotal);
                await db.Command.ExecuteNonQueryAsync();
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                db.Disconnect();
            }
        }

        public async Task<int> GetLastOrderId()
        {
            int result;

            try
            {
                db.Connect();
                db.Command.CommandText = "SELECT MAX(id) FROM orders";
                result = (int)await db.Command.ExecuteScalarAsync();
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.Message);
                result = -1;
            }
            finally
            {
                db.Disconnect();
            }

            return result;
        }
    }
}
