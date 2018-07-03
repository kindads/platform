using Captivate.Azure;
using Captivate.Common.Interfaces;
using Captivate.Comun.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Captivate.MoneyAds.API.Controllers
{
    public class MobileInfoController : ApiController, ITelemetria
    {
        public ITrace telemetria { set; get; }

        public MobileInfoController()
        {
            telemetria = new Trace();
        }

        // POST api/MobileInfo
        public HttpResponseMessage Post([FromBody] MobileInfo data)
        {
            bool result = false;
            try
            {
                if(data.Mobile!=null || data.Phone!=null || data.Tablet !=null)
                {
                    // Encolamos si tiene distinto de null la info
                    data.Ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";

                    string QueueName = ConfigurationManager.AppSettings["MobiInfoQueue"];
                    string dataString = JsonConvert.SerializeObject(data);
                    QueueManager.InsertMessage(dataString, QueueName);                    
                }
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