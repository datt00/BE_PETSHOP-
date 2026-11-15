
using BE_PetShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BE_PetShop.Controllers
{
    public class StoreController : Controller
    {
        // GET: Store
        public ActionResult Index()
        {
            var products = db.Products
        .Where(p => p.IsActive == true)
        .Select(p => new ProductCardViewModel
        {
            ProductID = p.ProductID,
            ProductName = p.ProductName,
            CategoryName = p.Category.CategoryName,
            ProductPrice = p.ProductPrice,
            ImageUrl = p.ImageUrl
        })
        .ToList();

            return View(products);
        }
        private BE_PetShopEntities db = new BE_PetShopEntities();
    }
}