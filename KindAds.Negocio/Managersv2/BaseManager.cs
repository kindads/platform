using KindAds.Azure;
using KindAds.Common.Interfaces;
using KindAdsV2.Azure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.Managersv2
{
    public class BaseManager : ITelemetria
    {
        //Todo
        public KindAdsV2DataAccess context { set; get; }
        public string endpointUrl { set; get; }
        public string primaryKey { set; get; }
        public ITrace telemetria { set; get; }
        public string databaseName { set; get; }
        public string collectionName { set; get; }

        public BaseManager(string endpointUrl, string primaryKey)
        {
            telemetria = new Trace();
            try
            {
                if (endpointUrl.Equals(string.Empty) || primaryKey.Equals(string.Empty))
                {
                    Inicialization();
                    context = new KindAdsV2DataAccess(this.endpointUrl, this.primaryKey);
                }
                else
                {
                    context = new KindAdsV2DataAccess(endpointUrl, primaryKey);
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

        }

        public BaseManager()
        {
            telemetria = new Trace();
            Inicialization();
            context = new KindAdsV2DataAccess(endpointUrl, primaryKey);
        }

        private void Inicialization()
        {
            try
            {
                this.endpointUrl = ConfigurationManager.AppSettings["azure-cosmos-endpoint"];
                this.primaryKey = ConfigurationManager.AppSettings["azure-cosmos-primarykey"];
                this.databaseName = ConfigurationManager.AppSettings["azure-cosmos-databasename"];
                this.collectionName = string.Empty;
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

        }
    }
}
