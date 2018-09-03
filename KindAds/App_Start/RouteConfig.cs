using KindAds.Controllers;
using KindAds.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KindAds {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                            name: "Default",
                            url: "{culture}/{controller}/{action}/{id}",
                            defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Home", id = UrlParameter.Optional }
                        );

            //Lobby for improve load speed, don´t remove please !! :)

            //routes.MapRoute(
            //                name: "Default",
            //                url: "{culture}/{controller}/{action}/{id}",
            //                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Lobby", id = UrlParameter.Optional }
            //            );
        }
    }
}
