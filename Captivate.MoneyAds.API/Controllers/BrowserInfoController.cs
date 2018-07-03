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
    public class BrowserInfoController : ApiController, ITelemetria
    {
        public ITrace telemetria { set; get; }

        public BrowserInfoController()
        {
            telemetria = new Trace();
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] BrowserInfo data)
        {
            bool result = false;
            try
            {
                // Encolamos si tiene distinto de null la info
                data.Ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";
                string QueueName = ConfigurationManager.AppSettings["BrowserInfoQueue"];
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