using KindAds.Business;
using KindAds.Negocio.Managersv2;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.WebJob.Audience
{
    public class Functions
    {
        private readonly AudienceManager _notificationManager;
        Dictionary<string, int> UserPriority { set; get; }


        public Functions()
        {
            UserPriority = new Dictionary<string, int>();
            _notificationManager = new AudienceManager();
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public void ProcessAudienceQueueMessage([QueueTrigger("%azure-queue-audience-changenotification%")] string message, TextWriter log)
        {
            
            _notificationManager.ProcessAudienceChangeNotification(message, UserPriority);
            _notificationManager.Close();
        }
    }
}
