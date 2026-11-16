using System;
using System.Linq;
using System.Web.Mvc;
using BE_PetShop.Models;

namespace BE_PetShop.Controllers
{
    public class AccountController : Controller
    {
        private BE_PetShopEntities db = new BE_PetShopEntities();

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string FullName, string Email, string Password, string Phone, string Address)
        {
            var checkEmail = db.Customers.FirstOrDefault(x => x.Email == Email);
            if (checkEmail != null)
            {
                ViewBag.Error = "Email này đã được sử dụng!";
                return View();
            }

            var customer = new Customer
            {
                FullName = FullName,
                Email = Email,
                Password = Password,
                Phone = Phone,
                Address = Address,
                CreatedDate = DateTime.Now
            };

            db.Customers.Add(customer);
            db.SaveChanges();

            Session["Customer"] = customer;
            return RedirectToAction("Index", "Store");
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string EmailOrUsername, string Password)
        {
            // 1. Kiểm tra tài khoản Admin (bảng User)
            var admin = db.Users.FirstOrDefault(x => (x.Username == EmailOrUsername || x.Email == EmailOrUsername) && x.Password == Password);
            if (admin != null)
            {
                Session["Admin"] = admin;
                return RedirectToAction("Index", "Products", new { area = "Admin" });
            }

            // 2. Kiểm tra tài khoản Khách hàng (bảng Customer)
            var customer = db.Customers.FirstOrDefault(x => x.Email == EmailOrUsername && x.Password == Password);
            if (customer != null)
            {
                Session["Customer"] = customer;
                return RedirectToAction("Index", "Store");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session["Customer"] = null;
            Session["Admin"] = null;
            return RedirectToAction("Index", "Store");
        }
    }
}