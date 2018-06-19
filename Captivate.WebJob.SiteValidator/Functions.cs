using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivate.Comun.Models;
using Captivate.Negocio;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace Captivate.WebJob.SiteValidator
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("%siteValidatorQueue%")] string message, TextWriter log)
        {
            SiteManager manager = new SiteManager();           
            manager.ValidateToken(message);
        }
    }
}
