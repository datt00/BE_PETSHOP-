using BE_PetShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BE_PetShop.Controllers
{
    public class CartController : Controller
    {
        private BE_PetShopEntities db = new BE_PetShopEntities();
        private List<CartItem> GetCart()
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        public ActionResult Index()
        {
            var cart = GetCart();
            ViewBag.TotalAmount = cart.Sum(x => x.SubTotal);
            return View(cart);
        }

        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductID == id);

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    ImageUrl = product.ImageUrl,
                    UnitPrice = product.ProductPrice ?? 0,
                    Quantity = quantity
                });
            }

            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductID == id);
            if (item != null)
            {
                if (quantity > 0)
                    item.Quantity = quantity;
                else
                    cart.Remove(item);
            }
            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }
        public ActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductID == id);
            if (item != null)
            {
                cart.Remove(item);
            }
            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        public ActionResult Checkout()
        {
            var cart = GetCart();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Store");
            }
            ViewBag.TotalAmount = cart.Sum(x => x.SubTotal);
            return View(cart);
        }

        // POST: Cart/Checkout (Lưu khách hàng + đơn hàng vào DB)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(string FullName, string Phone, string Address)
        {
            var cart = GetCart();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Store");
            }

            // 1. Tạo thông tin khách hàng trong bảng Customer
            var customer = new Customer
            {
                FullName = FullName,
                Email = "guest_" + DateTime.Now.Ticks + "@petshop.com", // Email tạm nếu khách chưa đăng nhập
                Password = "123",
                Phone = Phone,
                Address = Address,
                CreatedDate = DateTime.Now
            };
            db.Customers.Add(customer);
            db.SaveChanges();

            // 2. Tạo đơn hàng mới đầy đủ các cột trong bảng Order
            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(x => x.SubTotal),
                OrderStatus = "Chờ xác nhận",
                PaymentStatus = "Chưa thanh toán",
                ShippingAddress = Address,
                Phone = Phone
            };
            db.Orders.Add(order);
            db.SaveChanges();

            // 3. Lưu từng món vào bảng OrderDetail
            foreach (var item in cart)
            {
                // Tìm ProductDetail của sản phẩm, nếu chưa có thì tự tạo mới 1 dòng mặc định
                var prodDetail = db.ProductDetails.FirstOrDefault(x => x.ProductID == item.ProductID);
                if (prodDetail == null)
                {
                    prodDetail = new ProductDetail
                    {
                        ProductID = item.ProductID
                    };
                    db.ProductDetails.Add(prodDetail);
                    db.SaveChanges();
                }

                var detail = new OrderDetail
                {
                    OrderID = order.OrderID,
                    ProductDetailID = prodDetail.ProductDetailID,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    SubTotal = item.SubTotal
                };
                db.OrderDetails.Add(detail);
            }
            db.SaveChanges();

            // Xóa giỏ hàng sau khi đặt thành công
            Session["Cart"] = null;
            return RedirectToAction("OrderSuccess");
        }

        public ActionResult OrderSuccess()
        {
            return View();
        }
    }
}