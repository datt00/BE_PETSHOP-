using BE_PetShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BE_PetShop.Controllers
{
    public class StoreController : Controller
    {
        private BE_PetShopEntities db = new BE_PetShopEntities();

        // GET: Store (Danh sách + Lọc danh mục + Tìm kiếm)
        public ActionResult Index(int? categoryId, string search)
        {
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentSearch = search;

            var query = db.Products.Where(p => p.IsActive == true);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryID == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search) || p.Description.Contains(search));
            }

            var products = query
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

        // GET: Store/Details/5 (Chi tiết sản phẩm)
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = db.Products.FirstOrDefault(p => p.ProductID == id && p.IsActive == true);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }
    }
}