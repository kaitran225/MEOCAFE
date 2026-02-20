using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    static class CurrentUser
    {
        public static string username = "User2";

        public static void setCurrent(string username)
        {
            CurrentUser.username = username;
        }

    }
}
