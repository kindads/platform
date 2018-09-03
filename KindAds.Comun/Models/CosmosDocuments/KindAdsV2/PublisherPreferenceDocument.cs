using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class PublisherPreferenceDocument : KindAdsV2Document
    {
        public string PublisherProfileId { set; get; }

        public string IndustryId { set; get; }

        public bool Like { set; get; }

        public PublisherPreferenceDocument()
        {
            PublisherProfileId = string.Format("<NULL>");
            IndustryId = string.Format("<NULL>");
            Like = false;
        }
    }
}
