using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class PushCrewCampaignRequest
    {
        public string Name { set; get; }
        public string Text { set; get; }

        public string Url { set; get; }

        public string Image { set; get; }

        public string ApiKey { set; get; }
    }
}
