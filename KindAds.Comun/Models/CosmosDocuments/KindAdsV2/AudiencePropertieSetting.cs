using KindAds.Comun.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class AudiencePropertieSetting : KindAdsV2Document, ISetting
    {
        public string Name { set; get; }
        public string Value { set; get; }
        public string ProductId { set; get; }
        public string CampaignId { set; get; }
    }
}
