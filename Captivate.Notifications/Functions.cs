using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivate.Azure;
using Captivate.Comun.Models;
using Captivate.Negocio;
using Captivate.Negocio.Email;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace Captivate.Notifications
{
    public class Functions
    {
        NotificationManager notificationManager { set; get; }
        Dictionary<string,int> UserPriority { set; get; }

       
        public Functions()
        {
            UserPriority = new Dictionary<string, int>();
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public  void ProcessMailQueueMessage([QueueTrigger("%mailNotificationQueue%")] string message, TextWriter log)
        {
            notificationManager = new NotificationManager();
            notificationManager.ProcessMailNotification(message, UserPriority);
            notificationManager.Close();
        }

    }

   
}
