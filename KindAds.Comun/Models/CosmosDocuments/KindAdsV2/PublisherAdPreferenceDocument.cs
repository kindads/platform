using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class PublisherAdPreferenceDocument : KindAdsV2Document
    {
        public string PublisherProfileId { set; get; }

        public string ChannelId { set; get; }

        public PublisherAdPreferenceDocument()
        {
            PublisherProfileId = string.Format("<NULL>");
            ChannelId = string.Format("<NULL>");
        }
    }
}
