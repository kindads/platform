using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class ProposalDocument : KindAdsV2Document
    {
        public string AudienceChannelId { set; get; }
        public string AdvertiserProfileId { set; get; }

        public List<ProposalAnswerDocument> Questions { set; get; }

        public string AditionalNote { set; get; }

        public bool? AcceptedByPublisher { get; set; }
        public string RejectDetail { get; set; }

        public double Price { get; set; }

        public ProposalDocument()
        {
            Questions = new List<ProposalAnswerDocument>();
            AudienceChannelId = string.Format("<NULL>");
            AdvertiserProfileId = string.Format("<NULL>");
            AcceptedByPublisher = null;
            AditionalNote = string.Empty;
        }
    }
}
