using POS.Services.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Helpers
{
    public class Factory
    {
    

        static readonly IDao CurrentDao = new PostgreSqlDao();
        public static IDao GetIDAO()
        {
            return CurrentDao;
        }
    }
}
