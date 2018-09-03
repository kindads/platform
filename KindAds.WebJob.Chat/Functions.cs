using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Business;
using Microsoft.Azure.WebJobs;

namespace KindAds.WebJob.Chat
{
    public class Functions
    {
        NotificationManager notificationManager { set; get; }
        Dictionary<string, int> UserPriority { set; get; }


        public Functions()
        {
            UserPriority = new Dictionary<string, int>();
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public void ProcessMailQueueMessage([QueueTrigger("%azure-queue-chatnotification%")] string message, TextWriter log)
        {
            string serviceBusQueueName = "chat-topic";
            notificationManager = new NotificationManager(serviceBusQueueName);
            notificationManager.ProcessChatNotification(message, UserPriority);
            notificationManager.Close();
        }

    }
}
