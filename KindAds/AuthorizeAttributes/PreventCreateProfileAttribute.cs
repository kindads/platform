using KindAds.Comun.Enums;
using KindAds.Negocio.Managersv2;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace KindAds.AuthorizeAttributes {
    /// <summary>
    /// Filter atribute para validar que no se pueda tener acceso al menos que este completado el perfil
    /// </summary>
    public class PreventCreateProfileAttribute : System.Web.Mvc.AuthorizeAttribute //System.Web.Http.AuthorizeAttribute
        {
        private readonly PublisherProfileManager _publisherProfileManager = new PublisherProfileManager();
        private readonly AdvertiserProfileManager _advertiserProfileManager = new AdvertiserProfileManager();

       

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized) {
                return false;
            }
            
            if (httpContext.User.IsInRole(RoleEnum.Advertiser.ToString()) && _advertiserProfileManager.IsProfileCompleted(httpContext.User.Identity.GetUserId())) {
                return false;
            }
            else if (httpContext.User.IsInRole(RoleEnum.Publisher.ToString()) && _publisherProfileManager.IsProfileCompleted(httpContext.User.Identity.GetUserId())) {

                return false;
            }
            
            return true;
        }

        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {

            if (filterContext.HttpContext.User.Identity.IsAuthenticated) {
                string controller = string.Empty;
                string action = string.Empty;
          
                if (filterContext.HttpContext.User.IsInRole(RoleEnum.Advertiser.ToString())) {
                    controller = "Marketplace";
                    action = "Advertiser";
                }
                else if (filterContext.HttpContext.User.IsInRole(RoleEnum.Publisher.ToString()) ) {
                    controller = "Marketplace";
                    action = "Publisher";
                }
                else {
                    base.HandleUnauthorizedRequest(filterContext);
                }
                
                filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(
                            new System.Web.Routing.RouteValueDictionary(
                                new {
                                    controller = controller,
                                    action = action
                                })
                            );

            }
            else {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
