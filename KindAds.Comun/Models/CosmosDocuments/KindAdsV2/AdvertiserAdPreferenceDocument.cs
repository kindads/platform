using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class AdvertiserAdPreferenceDocument : KindAdsV2Document
    {
        public string AdvertiserProfileId { set; get; }

        public string ChannelId { set; get; }

        public AdvertiserAdPreferenceDocument()
        {
            AdvertiserProfileId = string.Format("<NULL>");
            ChannelId = string.Format("<NULL>");
        }
    }
}
