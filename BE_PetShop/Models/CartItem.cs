using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE_PetShop.Models
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal
        {
            get { return UnitPrice * Quantity; }
        }
    }
}