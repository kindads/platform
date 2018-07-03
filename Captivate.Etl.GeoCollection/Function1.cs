using System;
using Captivate.Comun.Interfaces;
using Captivate.Negocio.Etls;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Captivate.Etl.GeoCollection
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            IEtlManager manager = new EtlGeoLocationManager();
            manager.Execute();
        }
    }
}
