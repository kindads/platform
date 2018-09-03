using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class CampaignDocument : KindAdsV2Document
    {
        public string AudienceChannelId { set; get; }

        public string Name { set; get; }

        public string Text { set; get; }

        public string StartDate { set; get; }

        public string ProgressStatusId { set; get; }

        public string ApiCampaignId { set; get; }

        public string SettingId { set; get; }

        public bool Visibility { set; get; }

        public string Information { set; get; }
    }
}
