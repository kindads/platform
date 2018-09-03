using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace KindAds.WebJob.GeoTelemetry
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();
            config.Queues.MaxDequeueCount = 1;
            config.Queues.MaxPollingInterval = TimeSpan.FromSeconds(2);
            config.NameResolver = new QueueGeoInfoQueueNameResolver();

            var host = new JobHost(config);
            host.RunAndBlock();
        }

        public class QueueGeoInfoQueueNameResolver : INameResolver
        {
            public string Resolve(string name)
            {
                try
                {
                    string queueName = ConfigurationManager.AppSettings["GeoInfoQueue"].ToString();
                    return queueName;
                }
                catch (Exception e)
                {
                    //Todo
                }
                return "";
            }
        }
    }
}
