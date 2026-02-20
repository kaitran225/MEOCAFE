using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.OrderDao
{
    internal interface IOrderDao
    {
        Task AddOrder(Models.Order order);
        Task<int> GetLastOrderId();
    }
}
