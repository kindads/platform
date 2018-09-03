using KindAds.Azure;
using KindAds.Business;
using KindAds.Common.Interfaces;
using KindAds.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace KindAds.API.Controllers
{
    public class SiteController : ApiController , ITelemetria
    {
        public ITrace telemetria { set; get; }

        // [POST] api/site
        [System.Web.Http.HttpPost]
        public void Post(SiteValidationToken SiteData)
        {
            telemetria = new Trace();
            try
            {              
                if (SiteData != null)
                {
                    SiteData.Token = Guid.NewGuid().ToString();
                    SiteData.Ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";
                    SiteData.UserAgent = HttpContext.Current.Request.UserAgent;

                    string QueueName = ConfigurationManager.AppSettings["azure-queue-sitevalidator"];
                    string dataString = JsonConvert.SerializeObject(SiteData);
                    QueueManager.InsertMessage(dataString, QueueName);
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            
        }
    }
}
