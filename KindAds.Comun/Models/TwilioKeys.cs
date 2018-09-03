using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Models
{    
    public class TwilioKeys
    {
        public string SMSAccountIdentification { set; get; }
        public  string SMSAccountPassword { set; get; }
        public  string SMSAccountFrom { set; get; }

        public TwilioKeys()
        {
            SMSAccountIdentification = string.Empty; // "My Idenfitication"
            SMSAccountPassword = string.Empty; // "My Password";
            SMSAccountFrom = string.Empty; //"+15555551234";
        }
    }
}
