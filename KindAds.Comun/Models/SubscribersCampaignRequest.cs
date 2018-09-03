using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class SubscribersCampaignRequest
    {
        public string Name { set; get; }

        public string Text { set; get; }

        public string Url { set; get; }

        public string Image { set; get; }

        public string Utm { set; get; }

        public string UtmMedium { set; get; }

        public string UtmSource { set; get; }

        public string ApiKey { set; get; }
    }
}
