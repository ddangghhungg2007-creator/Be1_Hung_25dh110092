using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Be1_Hung_25dh110092.Models.ViewModel
{
    public class PersonalMenuVM
    {
        public bool IsLoggedIn { get; set; }

        public string Username { get; set; }

        public int CartCount { get; set; }
    }
}