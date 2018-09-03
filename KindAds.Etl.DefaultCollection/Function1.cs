using System;
using KindAds.Comun.Interfaces;
using KindAds.Negocio.Etls;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace KindAds.Etl.DefaultCollection
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            IEtlManager clickManager = new EtlDefaulClickManager();
            IEtlManager impressionManager = new EtlDefaulImpressionManager();

            clickManager.Execute();
            impressionManager.Execute();
        }
    }
}
