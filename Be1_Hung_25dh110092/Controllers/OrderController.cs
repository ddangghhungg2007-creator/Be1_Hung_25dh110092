using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Mvc;
using Be1_Hung_25dh110092.Models;
using Be1_Hung_25dh110092.Models.ViewModel;

namespace Be1_Hung_25dh110092.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Order/Checkout
        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as List<CartItem>;

            // Kiểm tra giỏ hàng
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // Lấy tài khoản đang đăng nhập
            var user = db.Users.FirstOrDefault(u =>
                u.Username == User.Identity.Name);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin khách hàng
            var customer = db.Customers.FirstOrDefault(c =>
                c.Username == user.Username);

            if (customer == null)
            {
                return HttpNotFound();
            }

            // Tạo dữ liệu hiển thị trang thanh toán
            CheckoutVM model = new CheckoutVM();

            model.CartItems = cart;
            model.CustomerID = customer.CustomerID;
            model.OrderDate = DateTime.Now;
            model.TotalAmount = cart.Sum(x => x.TotalPrice);
            model.AddressDelivery = customer.CustomerAddress;

            return View(model);
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            var cart = Session["Cart"] as List<CartItem>;

            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            if (string.IsNullOrWhiteSpace(model.AddressDelivery))
            {
                ModelState.AddModelError("AddressDelivery",
                    "Vui lòng nhập địa chỉ giao hàng.");
            }

            if (!ModelState.IsValid)
            {
                model.CartItems = cart;
                model.TotalAmount = cart.Sum(x => x.TotalPrice);
                return View(model);
            }

            var user = db.Users.FirstOrDefault(u =>
                u.Username == User.Identity.Name);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = db.Customers.FirstOrDefault(c =>
                c.Username == user.Username);

            if (customer == null)
            {
                return HttpNotFound();
            }

            // Tạo đơn hàng bằng các trường đang có
            Order order = new Order();

            order.CustomerID = customer.CustomerID;
            order.OrderDate = DateTime.Now;
            order.TotalAmount = cart.Sum(x => x.TotalPrice);
            order.PaymentStatus = "Chưa thanh toán";
            order.AddressDelivery = model.AddressDelivery.Trim();

            // Thêm đơn hàng
            db.Orders.Add(order);
            db.SaveChanges();

            // Thêm chi tiết đơn hàng
            foreach (var item in cart)
            {
                OrderDetail detail = new OrderDetail();

                detail.OrderID = order.OrderID;
                detail.ProductID = item.ProductID;
                detail.Quantity = item.Quantity;
                detail.UnitPrice = item.Price;

                db.OrderDetails.Add(detail);
            }

            db.SaveChanges();

            // Xóa giỏ hàng sau khi đặt thành công
            Session["Cart"] = null;

            return RedirectToAction("OrderSuccess",
                new { id = order.OrderID });
        }

        // GET: Order/OrderSuccess/5
        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(o =>
                    o.OrderID == id &&
                    o.Customer.User.Username == User.Identity.Name);
       

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Order/MyOrder
        public ActionResult MyOrder(string tuKhoa, string trangThai)
        {
            string username = User.Identity.Name;

            var customer = db.Customers.FirstOrDefault(c =>
                c.Username == username);

            if (customer == null)
            {
                return HttpNotFound();
            }

            var orders = db.Orders
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .Where(o => o.CustomerID == customer.CustomerID);

            // Tìm theo mã đơn hoặc tên sản phẩm
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                int maDon;

                if (int.TryParse(tuKhoa, out maDon))
                {
                    orders = orders.Where(o => o.OrderID == maDon);
                }
                else
                {
                    orders = orders.Where(o =>
                        o.OrderDetails.Any(d =>
                            d.Product.ProductName.Contains(tuKhoa)));
                }
            }

            // Lọc trạng thái thanh toán hiện có trong database
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                orders = orders.Where(o =>
                    o.PaymentStatus == trangThai);
            }

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TrangThai = trangThai;

            return View(orders
                .OrderByDescending(o => o.OrderDate)
                .ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}