using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Be1_Hung_25dh110092.Models.ViewModel
{
    public class CheckoutVM
    {
        public List<CartItem> CartItems { get; set; }

        public int CustomerID { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string AddressDelivery { get; set; }
    }
}