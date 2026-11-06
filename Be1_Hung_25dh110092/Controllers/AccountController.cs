using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Security;
using Be1_Hung_25dh110092.Models;
using Be1_Hung_25dh110092.Models.ViewModel;

namespace Be1_Hung_25dh110092.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên đăng nhập đã tồn tại
                bool daTonTai = db.Users.Any(u => u.Username == model.Username.Trim());

                if (daTonTai)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Tạo tài khoản
                User user = new User();
                user.Username = model.Username.Trim();
                user.Password = model.Password;
                user.UserRole = "C";

                // Tạo thông tin khách hàng
                Customer customer = new Customer();
                customer.CustomerName = model.CustomerName;
                customer.CustomerPhone = model.CustomerPhone;
                customer.CustomerEmail = model.CustomerEmail;
                customer.CustomerAddress = model.CustomerAddress;
                customer.Username = user.Username;

                // Lưu cả hai bảng
                db.Users.Add(user);
                db.Customers.Add(customer);
                db.SaveChanges();

                TempData["ThongBao"] = "Đăng ký tài khoản thành công!";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tài khoản khách hàng
                var user = db.Users.FirstOrDefault(u =>
                    u.Username == model.Username &&
                    u.Password == model.Password &&
                    u.UserRole == "C");

                if (user != null)
                {
                    // Lưu thông tin đăng nhập
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;

                    // Tạo Cookie xác thực
                    FormsAuthentication.SetAuthCookie(user.Username, false);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/ProfileInfo
        [Authorize]
        public ActionResult ProfileInfo()
        {
            string username = User.Identity.Name;

            var customer = db.Customers.FirstOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return HttpNotFound();
            }

            var model = new RegisterVM
            {
                Username = customer.Username,
                CustomerName = customer.CustomerName,
                CustomerPhone = customer.CustomerPhone,
                CustomerEmail = customer.CustomerEmail,
                CustomerAddress = customer.CustomerAddress
            };

            return View(model);
        }

        // POST: Account/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult UpdateProfile(RegisterVM model)
        {
            // Không bắt người dùng nhập lại mật khẩu khi cập nhật hồ sơ
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");

            if (ModelState.IsValid)
            {
                string username = User.Identity.Name;

                var customer = db.Customers.FirstOrDefault(c => c.Username == username);

                if (customer == null)
                {
                    return HttpNotFound();
                }

                customer.CustomerName = model.CustomerName;
                customer.CustomerPhone = model.CustomerPhone;
                customer.CustomerEmail = model.CustomerEmail;
                customer.CustomerAddress = model.CustomerAddress;

                db.SaveChanges();

                TempData["ThongBao"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("ProfileInfo");
            }

            return View("ProfileInfo", model);
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            string username = User.Identity.Name;

            var user = db.Users.FirstOrDefault(u =>
                u.Username == username &&
                u.Password == oldPassword);

            if (user == null)
            {
                TempData["Loi"] = "Mật khẩu hiện tại không đúng.";
                return RedirectToAction("ProfileInfo");
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6 || newPassword.Length > 50)
            {
                TempData["Loi"] = "Mật khẩu mới phải từ 6 đến 50 ký tự.";
                return RedirectToAction("ProfileInfo");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Loi"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToAction("ProfileInfo");
            }

            user.Password = newPassword;
            db.SaveChanges();

            TempData["ThongBao"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("ProfileInfo");
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