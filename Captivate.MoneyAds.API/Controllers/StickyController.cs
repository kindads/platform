using Captivate.Azure;
using Captivate.Common.Interfaces;
using Captivate.Comun.Models;
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
   
    public class StickyController : ApiController, ITelemetria
    {
        public ITrace telemetria { set; get; }

        public StickyController()
        {
            telemetria = new Trace();
        }

        [Route("api/Sticky/PostClicInfo")]
        [HttpPost]
        public HttpResponseMessage PostClicInfo([FromBody] StickyClicInfo data)
        {
            bool result = false;
            try
            {
                // Encolamos si tiene distinto de null la info
                data.Ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";
                string QueueName = ConfigurationManager.AppSettings["StickyClicInfoQueue"];
                string dataString = JsonConvert.SerializeObject(data);
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



        [Route("api/Sticky/PostImpressionInfo")]
        [HttpPost]
        public HttpResponseMessage PostImpressionInfo([FromBody] StickyImpressionInfo data)
        {
            bool result = false;
            try
            {
                // Encolamos si tiene distinto de null la info
                data.Ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";
                string QueueName = ConfigurationManager.AppSettings["StickyImpressionInfoQueue"];
                string dataString = JsonConvert.SerializeObject(data);
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