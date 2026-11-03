using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Be1_Hung_25dh110092.Models.ViewModel
{
    public class HomeProductVM
    {

        // Danh sách sản phẩm nổi bật
        public List<Product> FeaturedProducts { get; set; }

        // Danh sách sản phẩm mới
        public List<Product> NewProducts { get; set; }

        // Danh sách danh mục
        public List<Category> Categories { get; set; }

        // Từ khóa tìm kiếm
        public string SearchTerm { get; set; }
    }
}