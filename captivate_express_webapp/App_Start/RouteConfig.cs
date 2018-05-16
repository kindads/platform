using captivate_express_webapp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace captivate_express_webapp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                      name: "Default",
                      url: "{controller}/{action}/{id}",
                      defaults: new { controller = "Access", action = "Home", id = UrlParameter.Optional }
                  );
    }
  }
}
