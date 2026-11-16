using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BE_PetShop.Models;

namespace BE_PetShop.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private BE_PetShopEntities db = new BE_PetShopEntities();

        // GET: Admin/Orders (Danh sách đơn hàng)
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var orders = db.Orders.Include(o => o.Customer)
                                  .OrderByDescending(o => o.OrderDate)
                                  .ToList();
            return View(orders);
        }

        // GET: Admin/Orders/Details/5 (Xem chi tiết đơn hàng)
        public ActionResult Details(int? id)
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var order = db.Orders.Include(o => o.Customer)
                                 .Include(o => o.OrderDetails)
                                 .FirstOrDefault(o => o.OrderID == id);
            if (order == null) return HttpNotFound();

            return View(order);
        }

        // POST: Admin/Orders/UpdateStatus (Cập nhật trạng thái đơn)
        [HttpPost]
        public ActionResult UpdateStatus(int OrderID, string OrderStatus, string PaymentStatus)
        {
            var order = db.Orders.Find(OrderID);
            if (order != null)
            {
                order.OrderStatus = OrderStatus;
                order.PaymentStatus = PaymentStatus;
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = OrderID });
        }
    }
}