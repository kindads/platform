using KindAds.Azure;
using KindAds.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace KindAds.API
{
    public class WebApiApplication : System.Web.HttpApplication, ITelemetria
    {
        public ITrace telemetria { set; get; }
        protected void Application_Start()
        {
            try
            {
                telemetria = new Trace();
                AreaRegistration.RegisterAllAreas();
                GlobalConfiguration.Configure(WebApiConfig.Register);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
            catch (Exception ex)
            {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            telemetria = new Trace();
            Exception exception = Server.GetLastError();

            if (exception != null)
            {
                string ErrorId = Guid.NewGuid().ToString();
                string messageException = string.Format("ErrorId2 :{0} ,Details:{1}", ErrorId,
                    telemetria.MakeMessageException(exception, System.Reflection.MethodBase.GetCurrentMethod().Name)
                    );
                telemetria.Critical(messageException);
            }
        }
    }
}
