using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace POS.Models
{
    public class MenuItemDtoModel
    {
        [Name("id"), Optional]
        public int? Id { get; set; }

        [Name("name")]
        public string Name { get; set; }

        [Name("sell_price")]
        public decimal SellPrice { get; set; }
        [Name("category_id")]
        public int CategoryId { get; set; }

        [Name("capital_price")]
        public decimal CapitalPrice { get; set; }

        [Name("quantity")]
        public int Quantity { get; set; }
    }
}
