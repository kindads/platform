using Captivate.Azure;
using Captivate.Common.Interfaces;
using Captivate.Comun.Models.AdsMonetization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Captivate.MoneyAds.API.Controllers
{
    public class DefaultInfoController : ApiController, ITelemetria
    {
        public ITrace telemetria { set; get; }

        public DefaultInfoController()
        {
            telemetria=new Trace();
        }

        // api/Default
        public HttpResponseMessage Post([FromBody] DefaultClicInfo document)
        {
            bool result = false;
            try
            {
                // Encolamos si tiene distinto de null la info
                document.Ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";
                string QueueName = ConfigurationManager.AppSettings["DefaultClicInfoQueue"];
                string dataString = JsonConvert.SerializeObject(document);
                QueueManager.InsertMessage(dataString, QueueName);
                result = true;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }


            if (result)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
            }
        }
    }
}