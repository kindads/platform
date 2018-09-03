using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class OneSignalCampaignRequest
    {
        public string Name { set; get; }

        public string Text { set; get; }

        public string AppKey { set; get; }

        public string AppId { set; get; }
    }
}
