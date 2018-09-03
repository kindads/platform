using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace KindAds.WebJob.CampaignValidator
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();
            config.Queues.MaxPollingInterval = TimeSpan.FromSeconds(2);
            config.NameResolver = new QueueCampaignNameResolver();
            var host = new JobHost(config);
            host.RunAndBlock();
        }

        //azure-queue-campaign
    }

    public class QueueCampaignNameResolver : INameResolver
    {
        public string Resolve(string name)
        {
            string QueueName= ConfigurationManager. AppSettings["azure-queue-campaign"].ToString();
            return QueueName;
        }
    }
}
