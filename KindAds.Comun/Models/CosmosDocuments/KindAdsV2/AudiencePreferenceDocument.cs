using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class AudiencePreferenceDocument : KindAdsV2Document
    {
        public string AudienceId { set; get; }
        public string IndustryId { set; get; }
        public bool Like { set; get; }

        public AudiencePreferenceDocument()
        {
            AudienceId = string.Empty;
            IndustryId = string.Empty;
            Like = false;
        }
    }
}
