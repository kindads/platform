using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{   
    public class AdvertiserPreferenceDocument : KindAdsV2Document
    {
        public string AdvertiserProfileId { set; get; }
        public string IndustryId { set; get; }
        public bool Like { set; get; }

        public AdvertiserPreferenceDocument()
        {
            AdvertiserProfileId = string.Empty;
            IndustryId = string.Empty;
            Like = false;
        }
    }
}
