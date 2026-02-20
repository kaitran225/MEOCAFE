using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name;
        public decimal SellPrice;
        public int Quantity;
        public byte[]? Image { get; set; }
    }
}
