﻿using KindAds.Azure;
using KindAds.Business;
using KindAds.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace KindAds.API.Controllers
{
    public class CampaignController : ApiController, ITelemetria
    {
        public ITrace telemetria { set; get; }

        public CampaignController()
        {
            telemetria = new Trace();
        }


        // GET: Campaign
        public int Get(string idCampaign)
        {
            int result = 0;
            CampaignManager manager = new CampaignManager();
            try
            {
                var sendResult=manager.ValidateSettings(idCampaign);
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