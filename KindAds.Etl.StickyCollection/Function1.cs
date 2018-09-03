using System;
using KindAds.Comun.Interfaces;
using KindAds.Negocio.Etls;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace KindAds.Etl.StickyCollection
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            IEtlManager clickManager = new EtlStickyClickManager();
            IEtlManager impressionManager = new EtlStickyImpressionManager();

            clickManager.Execute();
            impressionManager.Execute();
        }
    }
}
