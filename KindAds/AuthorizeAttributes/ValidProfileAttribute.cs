using KindAds.Comun.Enums;
using KindAds.Negocio.Managersv2;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace KindAds.AuthorizeAttributes {
    public class ValidProfileAttribute : System.Web.Mvc.AuthorizeAttribute //System.Web.Http.AuthorizeAttribute
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
                return true;
            }
            else if (httpContext.User.IsInRole(RoleEnum.Publisher.ToString()) && _publisherProfileManager.IsProfileCompleted(httpContext.User.Identity.GetUserId())) {

                return true;
            }
            
            return false;
        }

        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {

            if (filterContext.HttpContext.User.Identity.IsAuthenticated) {
                string controller = string.Empty;
                string action = string.Empty;

                if (filterContext.HttpContext.User.IsInRole(RoleEnum.Advertiser.ToString())) {
                    controller = "Advertiserprofile";
                    action = "createprofile";
                }
                else if (filterContext.HttpContext.User.IsInRole(RoleEnum.Publisher.ToString()) ) {
                    controller = "Publisherprofile";
                    action = "createprofile";
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
