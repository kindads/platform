using KindAds.Comun.Interfaces;
using KindAds.Migration.ToCosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;

namespace KindAds.Etl.MigrateSqlToCosmosUsersCollection
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            IEtlManager manager = new MigrateUsersToCosmosETL();
            manager.Execute();
        }
    }
}
