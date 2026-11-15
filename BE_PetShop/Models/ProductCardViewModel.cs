using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE_PetShop.Models
{
    public class ProductCardViewModel
    {
        public int ProductID { get; set; } // mã sản phẩm
        public string ProductName { get; set; } // tên sản phẩm
        public string CategoryName { get; set; } // tên danh mục
        public decimal ProductPrice { get; set; } // đơn giá
        public string ImageUrl { get; set; } // tên file ảnh trong Content/images


    }
}