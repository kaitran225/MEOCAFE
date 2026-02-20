using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using POS.Services.Dao;

namespace POS.Services.OrderDetailDao
{
    public class OrderDetailDao : IOrderDetailDao
    {
        private readonly DatabaseManager db;

        public OrderDetailDao()
        {
            db = new DatabaseManager();
        }

        public async Task AddOrderDetail(Models.OrderDetail orderDetail)
        {

            try
            {
                db.Connect();
                db.Command.CommandText = "INSERT INTO order_details (id, order_id, menu_item_id, quantity, price, is_combo) VALUES (@id, @order_id, @menu_item_id, @quantity, @price, @is_combo)";
                db.Command.Parameters.AddWithValue("id", orderDetail.Id);
                db.Command.Parameters.AddWithValue("order_id", orderDetail.Order.Id);
                db.Command.Parameters.AddWithValue("menu_item_id", orderDetail.MenuItem.Id);
                db.Command.Parameters.AddWithValue("quantity", orderDetail.Quantity);
                db.Command.Parameters.AddWithValue("price", orderDetail.Price);
                db.Command.Parameters.AddWithValue("is_combo", orderDetail.MenuItem.IsCombo);
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

        public async Task<int> GetLastOrderDetailId()
        {
            int result;

            try
            {
                db.Connect();
                db.Command.CommandText = "SELECT MAX(id) FROM order_details";
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
