using KindAds.Azure;
using KindAds.Common.Interfaces;
using KindAds.Controllers;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace KindAds {
    public class MvcApplication : HttpApplication, ITelemetria
    {
        public ITrace telemetria { set; get; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            telemetria = new Trace();
            Exception exception = Server.GetLastError();

            if (exception != null) {
                string ErrorId = Guid.NewGuid().ToString();
                string messageException = string.Format("ErrorId:{0} ,Details:{1}", ErrorId,
                    telemetria.MakeMessageException(exception, System.Reflection.MethodBase.GetCurrentMethod().Name)
                    );
                telemetria.Critical(messageException);

                TempDataDictionary errors = new TempDataDictionary();
                errors.Add("ErrorId", ErrorId);

                var httpContext = ((HttpApplication)sender).Context;
                httpContext.Response.Clear();
                httpContext.ClearError();
                httpContext.Response.TrySkipIisCustomErrors = true;

                InvokeErrorAction(httpContext, exception);

            }
        }

        void InvokeErrorAction(HttpContext httpContext, Exception exception)
        {
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "InternalServer";
            routeData.Values["exception"] = exception;
            using (var controller = new ErrorController()) {
                ((IController)controller).Execute(
                new RequestContext(new HttpContextWrapper(httpContext), routeData));
            }
        }
    }
}
