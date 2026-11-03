using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Be1_Hung_25dh110092.Models.ViewModel
{
    public class ProductDetailVM
    {
        // Sản phẩm đang xem
        public Product Product { get; set; }

        // Các sản phẩm cùng danh mục
        public List<Product> RelatedProducts { get; set; }

        // Các sản phẩm bán chạy
        public List<Product> TopProducts { get; set; }

        // Số lượng muốn mua
        public int Quantity { get; set; } = 1;
    }
}
