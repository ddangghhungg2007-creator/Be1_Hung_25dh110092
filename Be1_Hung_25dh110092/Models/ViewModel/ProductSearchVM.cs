//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using PagedList;

//namespace Be1_Hung_25dh110092.Models.ViewModel
//{
//    //public class ProductSearchVM
//    //{
//    //    public string SearchTerm { get; set; }
//    //    public List<Product> Products { get; set; }

//    //}

//    //public class ProductSearchVM
//    //{
//    //    // Từ khóa tìm kiếm
//    //    public string SearchTerm { get; set; }

//    //    // Lọc theo khoảng giá
//    //    public decimal? MinPrice { get; set; }
//    //    public decimal? MaxPrice { get; set; }

//    //    // Sắp xếp
//    //    public string SortOrder { get; set; }

//    //    // Phân trang
//    //    public int PageNumber { get; set; }
//    //    public int PageSize { get; set; } = 2;

//    //    // Danh sách sản phẩm
//    //    public List<Product> Products { get; set; }
//    //}
//    public class ProductSearchVM
//    {
//        public string SearchTerm { get; set; }

//        public decimal? MinPrice { get; set; }

//        public decimal? MaxPrice { get; set; }

//        public string SortOrder { get; set; }

//        public int PageNumber { get; set; }

//        public int PageSize { get; set; } = 2;

//        public IPagedList<Product> Products { get; set; }
//    }

//}

using Be1_Hung_25dh110092.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace Be1_Hung_25dh110092.Models.ViewModel
{
    public class ProductSearchVM
    {
        public string SearchTerm { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string SortOrder { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; } = 2;

        public IPagedList<Product> Products { get; set; }
    }
}