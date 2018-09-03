using KindAds.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Models
{
    

    public class MailMessage  
    {
        public MailMessage()
        {
            Destination = string.Empty;
            Subject = "[SUBJECT]";
            Body = string.Empty;
        }
             
        public  string Destination { get; set; }

        public  string Subject { get; set; }

        public  string Body { get; set; }
    }
}
