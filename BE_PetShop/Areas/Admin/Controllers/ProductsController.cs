using BE_PetShop.Models;
using BE_PetShop.Models.ViewModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace BE_PetShop.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private BE_PetShopEntities db = new BE_PetShopEntities();

        public ActionResult Index(string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var model = new ProductSearchVM();
            var products = db.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))

                if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                                   p.ProductName.Contains(searchTerm) ||
                                   p.Description.Contains(searchTerm) ||
                                   p.Category.CategoryName.Contains(searchTerm));
                }

            //Tim kiem dua tren gia toi thieu
            if (minPrice.HasValue)
            {
                model.MinPrice = minPrice.Value;
                products = products.Where(p => p.ProductPrice >= minPrice.Value);
            }
            //Tim kiem dua tren gia toi da
            if (maxPrice.HasValue)
            {
                model.MaxPrice = maxPrice.Value;

                products = products.Where(p => p.ProductPrice <= maxPrice.Value);
            }
           
            //Áp dụng sắp xếp dựa trên lựa chọn của người dùng
            switch (sortOrder)
            {
                case "name_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                default: //Mặc định sắp xếp theo tên
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }
            model.SortOrder = sortOrder;
            int pageNumber = page ?? 1;
            int pageSize = 2;
            model.Products = products.ToPagedList(pageNumber, pageSize);

            return View(model);
        }
        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductPrice,ImageUrl,Description,IsActive")] Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // 1. Xử lý lưu file ảnh nếu người dùng có chọn ảnh
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                    ImageFile.SaveAs(path);
                    product.ImageUrl = fileName; // Lưu tên file vào DB
                }

                // 2. Đảm bảo IsActive có giá trị mặc định là true để tránh lỗi NOT NULL
                if (product.IsActive == null)
                {
                    product.IsActive = true;
                }

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }
        //Get: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();

            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }
        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductPrice,ImageUrl,Description,IsActive")] Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                    ImageFile.SaveAs(path);
                    product.ImageUrl = fileName;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);   
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose(); base.Dispose(disposing);
        }
    }
}