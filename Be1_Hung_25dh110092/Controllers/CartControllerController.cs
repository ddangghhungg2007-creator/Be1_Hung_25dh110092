using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Be1_Hung_25dh110092.Models;
using Be1_Hung_25dh110092.Models.ViewModel;


namespace Be1_Hung_25dh110092.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Lấy danh sách giỏ hàng từ Session
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

        // GET: Cart/Index
        public ActionResult Index()
        {
            var cart = GetCart();

            return View(cart);
        }

        // GET: Cart/AddToCart
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            if (quantity < 1)
            {
                quantity = 1;
            }

            // Tìm sản phẩm trong database
            var product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var cart = GetCart();

            // Kiểm tra sản phẩm đã có trong giỏ chưa
            var item = cart.FirstOrDefault(
                x => x.Product.ProductID == id
            );

            if (item != null)
            {
                // Đã có thì cộng thêm số lượng
                item.Quantity += quantity;
            }
            else
            {
                // Chưa có thì thêm mặt hàng mới
                //cart.Add(new CartItem
                //{
                //    Product = product,
                //    Quantity = quantity
                //});

                // Chưa có thì thêm mặt hàng mới
                cart.Add(new CartItem
                {
                    Product = product,
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    Price = product.ProductPrice,
                    Image = product.ProductImage,
                    Quantity = quantity
                });



            }

            Session["Cart"] = cart;

            return RedirectToAction("Index");
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(
                x => x.Product.ProductID == id
            );

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }

            Session["Cart"] = cart;

            return RedirectToAction("Index");
        }

        // POST: Cart/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(
                x => x.Product.ProductID == id
            );

            if (item != null)
            {
                cart.Remove(item);
            }

            Session["Cart"] = cart;

            return RedirectToAction("Index");
        }

        // POST: Cart/ClearCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClearCart()
        {
            Session["Cart"] = new List<CartItem>();

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