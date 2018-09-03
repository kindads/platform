using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Partners.Mail.SendinBlue
{
    public class SendingBlueCampaignResponse
    {
        public string code { set; get; }
        public string message { set; get; }
        public Data data { set; get; }
    }

    public class Data
    {
        public string id { set; get; }
    }
}
