using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class CampaignMonitorRequest
    {
        public string Name { set; get; }

        public string Text { set; get; }

        public string ApiKey { set; get; }

        public string ListId { set; get; }

        public string SegmentIDs { set; get; }

        public string ClientID { set; get; }

        public string Subject { set; get; }

        public string FromName { set; get; }

        public string FromEmail { set; get; }

        public CampaignDocument campaign { set; get; }
    }
}
