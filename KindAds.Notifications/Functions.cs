using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Azure;
using KindAds.Common.Models;
using KindAds.Negocio;
using KindAds.Business.Email;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using KindAds.Business;

namespace KindAds.Notifications
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
        public  void ProcessMailQueueMessage([QueueTrigger("%azure-queue-mailnotification%")] string message, TextWriter log)
        {
            notificationManager = new NotificationManager();
            notificationManager.ProcessMailNotification(message, UserPriority);
            notificationManager.Close();
        }

    }

   
}
