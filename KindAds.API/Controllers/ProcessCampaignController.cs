using KindAds.Azure;
using KindAds.Business;
using KindAds.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KindAds.API.Controllers
{
    public class ProcessCampaignController : ApiController, ITelemetria
    {
        public ITrace telemetria { set; get; }

        public ProcessCampaignController()
        {
            telemetria = new Trace();
        }


        // GET: Campaign
        public int Get(string idCampaign)
        {
            int result = 0;            
            try
            {
                CampaignManager manager = new CampaignManager();
                var sendResult = manager.Send(idCampaign);
                result = sendResult == true ? 1 : 0;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return result;
        }

    }
}