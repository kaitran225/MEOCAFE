using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class ShiftStatistic
    {
        public string employees_username { get; set; }
        public int total { get; set; }
        public int late { get; set; }
        public int on_time { get; set; }

    }
}
