using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivate.Comun.Models;
using Captivate.Negocio;
using Microsoft.Azure.WebJobs;

namespace Captivate.WebJob.GeoTelemetry
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("%GeoInfoQueue%")] string message, TextWriter log)
        {
            KindAdsTelemetryManager manager = new KindAdsTelemetryManager();
            string DatabaseName = ConfigurationManager.AppSettings["CosmosDatabaseName"];
            string CollectionName = ConfigurationManager.AppSettings["CosmosGeoCollectionName"];

            GeoInfo info = manager.GetInfo<GeoInfo>(message);
            manager.SendToCosmos(info, DatabaseName, CollectionName);
        }
    }
}
