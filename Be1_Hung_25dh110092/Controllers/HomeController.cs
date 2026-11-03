using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Be1_Hung_25dh110092.Models;
using Be1_Hung_25dh110092.Models.ViewModel;


namespace Be1_Hung_25dh110092.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
     

        private MyStoreEntities db = new MyStoreEntities();

        // GET: Home/Index
        public ActionResult Index(string searchTerm)
        {
            var products = db.Products.AsQueryable();

            // Tìm kiếm theo tên sản phẩm hoặc mô tả
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductDecription.Contains(searchTerm));
            }

            // Lấy 10 sản phẩm nổi bật
            var featuredProducts = products
                .OrderByDescending(p =>
                    p.OrderDetails.Sum(o => (int?)o.Quantity) ?? 0)
                .Take(10)
                .ToList();

            // Lấy 20 sản phẩm mới
            var newProducts = products
                .OrderByDescending(p => p.ProductID)
                .Take(20)
                .ToList();

            var model = new HomeProductVM
            {
                FeaturedProducts = featuredProducts,
                NewProducts = newProducts,
                Categories = db.Categories.ToList(),
                SearchTerm = searchTerm
            };

            return View(model);
        }

        // GET: Home/ProductList
        public ActionResult ProductList(int? id, string searchTerm)
        {
            var products = db.Products.AsQueryable();

            // Lọc theo danh mục
            if (id.HasValue)
            {
                products = products.Where(p => p.CategoryID == id.Value);
            }

            // Tìm kiếm sản phẩm
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductDecription.Contains(searchTerm));
            }

            ViewBag.CategoryName = id.HasValue
                ? db.Categories
                    .Where(c => c.CategoryID == id.Value)
                    .Select(c => c.CategoryName)
                    .FirstOrDefault()
                : "Tất cả sản phẩm";

            ViewBag.SearchTerm = searchTerm;

            return View(products.OrderBy(p => p.ProductID).ToList());
        }

        // GET: Home/ProductDetail
        public ActionResult ProductDetail(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var model = new ProductDetailVM
            {
                Product = product,

                // Lấy tối đa 8 sản phẩm cùng danh mục
                RelatedProducts = db.Products
                    .Where(p => p.CategoryID == product.CategoryID
                             && p.ProductID != product.ProductID)
                    .Take(8)
                    .ToList(),

                // Lấy tối đa 8 sản phẩm bán chạy
                TopProducts = db.Products
                    .OrderByDescending(p =>
                        p.OrderDetails.Sum(o => (int?)o.Quantity) ?? 0)
                    .Take(8)
                    .ToList(),

                Quantity = 1
            };

            return View(model);
        }

        // POST: Home/ProductDetail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductDetail(int id, int quantity)
        {
            if (quantity < 1)
            {
                quantity = 1;
            }

            // Chuyển sản phẩm sang giỏ hàng
            return RedirectToAction(
                "AddToCart",
                "Cart",
                new { id = id, quantity = quantity }
            );
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