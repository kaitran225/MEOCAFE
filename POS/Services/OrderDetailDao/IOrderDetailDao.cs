using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.OrderDetailDao
{
    internal interface IOrderDetailDao
    {
        Task AddOrderDetail(Models.OrderDetail orderDetail);
        Task<int> GetLastOrderDetailId();
    }
}
