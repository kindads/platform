using Captivate.Azure;
using Captivate.Common.Interfaces;
using Captivate.Comun.Enums;
using Captivate.Comun.Models;
using Captivate.Comun.Models.CosmosDocuments;
using Captivate.Negocio;
using Newtonsoft.Json;
using System.Configuration;
using System.Web;
using System.Web.Http;

namespace Captivate.MoneyAds.API.Controllers
{
    public class ConfigurationController : ApiController, ITelemetria
    {
        public ITrace telemetria { set; get; }

        public ConfigurationController()
        {
            telemetria = new Trace();
        }

        // GET api/Configurarion?IdUser={value}&IdSite={}
        public MoneyAdsConfig Get(string IdUser,string IdSite)
        {
            MoneyAdsConfig config = new MoneyAdsConfig();
            config.IdUser = IdUser;
            return config;
        }

        // GET api/Configurarion/GetInyectConfig?IdUser={value}&IdSite={}
        [Route("api/Configuration/GetInyectConfig")]
        [HttpGet]
        public InyectConfig GetInyectConfig(string IdUser,string IdSite)
        {
            InyectConfig configuration = new InyectConfig();
            try
            {
                KindAdsTelemetryManager manager = new KindAdsTelemetryManager();
                configuration = manager.GetInyectConfig(IdUser.ToUpper(),IdSite);

                // enqueue data
                EnqueueImpressionDefaultAndSticky(IdUser, IdSite);
            }
            catch (System.Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }           
            return configuration;
        }


        private bool EnqueueImpressionDefaultAndSticky(string IdUser,string IdSite)
        {
            bool result = false;
            try
            {
                CosmosDbDocument defaultInfo = new CosmosDbDocument();
                CosmosDbDocument stickyInfo = new CosmosDbDocument();

                defaultInfo.Ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";
                stickyInfo.Ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";

                defaultInfo.IdSite = IdSite;
                stickyInfo.IdSite = IdSite;

                defaultInfo.IdUser = IdUser;
                stickyInfo.IdUser = IdUser;

                defaultInfo.Metric= AdsMonetizationMetrics.Impression;
                stickyInfo.Metric= AdsMonetizationMetrics.Impression;

                defaultInfo.Type = AdsMonetizationTypes.Default;
                stickyInfo.Type = AdsMonetizationTypes.Sticky;


                var resultDafault = EnqueueImpressionDefault(defaultInfo);
                var resultSticky = EnqueueImpressionSticky(stickyInfo);

                result = (resultDafault == true && resultSticky == true) ? true : false;
            }
            catch (System.Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }


        private bool EnqueueImpressionDefault(CosmosDbDocument document)
        {
            bool result = false;
            try
            {
                string QueueName = ConfigurationManager.AppSettings["DefaultImpressionInfoQueue"];
                string dataString = JsonConvert.SerializeObject(document);
                QueueManager.InsertMessage(dataString, QueueName);
                result = true;
            }
            catch (System.Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }

        private bool EnqueueImpressionSticky(CosmosDbDocument document)
        {
            bool result = false;
            try
            {
                string QueueName = ConfigurationManager.AppSettings["StickyImpressionInfoQueue"];
                string dataString = JsonConvert.SerializeObject(document);
                QueueManager.InsertMessage(dataString, QueueName);
                result = true;
            }
            catch (System.Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }
    }
}