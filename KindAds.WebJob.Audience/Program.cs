using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.WebJob.Audience
{
    public class Program
    {
        static void Main()
        {
            var config = new JobHostConfiguration();
            config.Queues.MaxDequeueCount = 1;
            config.Queues.MaxPollingInterval = TimeSpan.FromSeconds(2);
            config.NameResolver = new AudienceChangeNotificationNameResolver();

            var host = new JobHost(config);
            host.RunAndBlock();
        }
    }

    public class AudienceChangeNotificationNameResolver : INameResolver
    {
        public string Resolve(string name)
        {
            return ConfigurationManager.AppSettings["azure-queue-audience-changenotification"].ToString();
        }
    }
}
