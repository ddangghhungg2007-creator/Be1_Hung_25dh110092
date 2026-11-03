using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Be1_Hung_25dh110092.Models
{
    public class CartItem
    {
        public decimal TotalPrice
        {
            get { return Price * Quantity; }
        }
        public Product Product { get; set; }

        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Image { get; set; }
    }
}