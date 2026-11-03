using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Be1_Hung_25dh110092.Models;

namespace Be1_Hung_25dh110092.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Customers
        public ActionResult Index(string tuKhoa)
        {
            var customers = db.Customers.Include(c => c.User);

            // Tìm kiếm khách hàng
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                customers = customers.Where(c =>
                    (c.CustomerName ?? "").Contains(tuKhoa) ||
                    (c.CustomerPhone ?? "").Contains(tuKhoa) ||
                    (c.CustomerEmail ?? "").Contains(tuKhoa));
            }

            ViewBag.TuKhoa = tuKhoa;

            return View(customers.OrderBy(c => c.CustomerID).ToList());
        }

        // GET: Admin/Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Customer customer = db.Customers.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // GET: Admin/Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customers/Create
        // POST: Admin/Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "CustomerName,CustomerPhone,CustomerEmail,CustomerAddress")] Customer customer,
            string TaiKhoanMoi,
            string MatKhauMoi)
        {
            // NOTE: Chuẩn hóa tên đăng nhập
            TaiKhoanMoi = (TaiKhoanMoi ?? "").Trim();

            // NOTE: Kiểm tra họ tên
            if (customer == null)
            {
                ModelState.AddModelError("", "Thông tin khách hàng không hợp lệ.");
                return View();
            }

            if (string.IsNullOrWhiteSpace(customer.CustomerName))
            {
                ModelState.AddModelError("CustomerName", "Vui lòng nhập họ và tên.");
            }

            // NOTE: Kiểm tra số điện thoại
            if (string.IsNullOrWhiteSpace(customer.CustomerPhone))
            {
                ModelState.AddModelError("CustomerPhone", "Vui lòng nhập số điện thoại.");
            }
            else if (customer.CustomerPhone.Length > 15)
            {
                ModelState.AddModelError("CustomerPhone", "Số điện thoại tối đa 15 ký tự.");
            }

            // NOTE: Kiểm tra tên đăng nhập
            if (string.IsNullOrWhiteSpace(TaiKhoanMoi))
            {
                ModelState.AddModelError("TaiKhoanMoi", "Vui lòng nhập tên đăng nhập.");
            }
            else if (TaiKhoanMoi.Length > 255)
            {
                ModelState.AddModelError("TaiKhoanMoi", "Tên đăng nhập tối đa 255 ký tự.");
            }
            else if (db.Users.Any(u => u.Username == TaiKhoanMoi))
            {
                ModelState.AddModelError("TaiKhoanMoi", "Tên đăng nhập đã tồn tại.");
            }

            // NOTE: Kiểm tra mật khẩu
            if (string.IsNullOrWhiteSpace(MatKhauMoi))
            {
                ModelState.AddModelError("MatKhauMoi", "Vui lòng nhập mật khẩu.");
            }
            else if (MatKhauMoi.Length > 50)
            {
                ModelState.AddModelError("MatKhauMoi", "Mật khẩu tối đa 50 ký tự.");
            }

            // NOTE: Nếu dữ liệu hợp lệ thì tiến hành lưu
            if (ModelState.IsValid)
            {
                try
                {
                    // Tạo tài khoản mới
                    User user = new User();

                    user.Username = TaiKhoanMoi;
                    user.Password = MatKhauMoi;

                    // NOTE: Giữ mã K theo code hiện tại.
                    // Cần đảm bảo K là mã vai trò hợp lệ trong database.
                    user.UserRole = "C";

                    // NOTE: Liên kết khách hàng với tài khoản vừa tạo
                    customer.Username = user.Username;
                    customer.User = user;

                    // Thêm tài khoản và khách hàng
                    db.Users.Add(user);
                    db.Customers.Add(customer);

                    // Lưu dữ liệu vào database
                    db.SaveChanges();

                    TempData["ThongBao"] = "Thêm khách hàng và tài khoản thành công!";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // NOTE: Lấy nguyên nhân lỗi thực tế
                    string loi = ex.Message;

                    Exception loiTrong = ex.InnerException;

                    while (loiTrong != null)
                    {
                        loi += " | Chi tiết: " + loiTrong.Message;
                        loiTrong = loiTrong.InnerException;
                    }

                    ModelState.AddModelError("", "LỖI LƯU KHÁCH HÀNG: " + loi);
                }
            }

            return View(customer);
        }

        // GET: Admin/Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Customer customer = db.Customers.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // POST: Admin/Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "CustomerID,CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username")] Customer customer)
        {
            // NOTE: Kiểm tra tài khoản liên kết có tồn tại không
            if (string.IsNullOrWhiteSpace(customer.Username) ||
                !db.Users.Any(u => u.Username == customer.Username))
            {
                ModelState.AddModelError("Username", "Tài khoản không tồn tại.");
            }

            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Admin/Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Customer customer = db.Customers.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            db.Customers.Remove(customer);
            db.SaveChanges();

            return RedirectToAction("Index");
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