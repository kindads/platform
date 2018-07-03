using System.Web;
using System.Web.Mvc;

namespace Captivate.MoneyAds.API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
