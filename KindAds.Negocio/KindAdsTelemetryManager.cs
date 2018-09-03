using KindAds.Azure;
using KindAds.Common.Interfaces;
using KindAds.Comun.Models;
using KindAds.Comun.Models.CosmosDocuments;
using KindAds.DataAccess.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio
{
    public class KindAdsTelemetryManager: ITelemetria
    {
        public ITrace telemetria { set; get; }
        public CosmosDBManager manager { set; get; }

        public KindAdsTelemetryManager()
        {
            telemetria = new Trace();
            manager = new CosmosDBManager();
        }


        public T GetInfo<T>(string message) where T : CosmosDbDocument, new()
        {
            T info = new T();
            try
            {
               info = JsonConvert.DeserializeObject<T>(message);
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }            
            return info;
        }

        public bool SendToCosmos<T>(T info, string DatabaseName, string CollectionName) where T : CosmosDbDocument
        {
            throw new NotImplementedException();
            //bool result = false;
            //try
            //{                
            //    bool resultCreateCollection = manager.CreateCollectionIfNotExist(DatabaseName, CollectionName);
            //    result = manager.CreateDocumentIfNotExists<T>(DatabaseName, CollectionName, info);
            //}
            //catch (Exception e)
            //{
            //    var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //}
            //return result;
        }

        public InyectConfig GetInyectConfig(string IdUser, string IdSite)
        {
            InyectConfig configuration = new InyectConfig();
            PublisherAdsRepository repository = new PublisherAdsRepository();

            try
            {
                configuration.defaultConfig = repository.GetDefaultConfig(IdUser, IdSite);
                configuration.stickyConfig = repository.GetStickyConfig(IdUser, IdSite);
                repository.GetStickyHtml(configuration.stickyConfig);
            
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }           

            return configuration;            
        }
    }
}
