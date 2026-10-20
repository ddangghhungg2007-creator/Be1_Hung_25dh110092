using System.Web;
using System.Web.Mvc;

namespace Be1_Hung_25dh110092
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
