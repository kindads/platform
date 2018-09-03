using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class PublisherPreferenceQuestionDocument : KindAdsV2Document
    {
        public string CampaignId { set; get; }
        public string Question { set; get; }
        public string Answer { set; get; }

        public PublisherPreferenceQuestionDocument(string question)
        {
            Id = Guid.NewGuid().ToString();
            Question = question;
            Answer = string.Format("<NULL>");
        }

        public PublisherPreferenceQuestionDocument()
        {
            CampaignId = string.Format("<NULL>");
            Question = string.Format("<NULL>");
            Answer = string.Format("<NULL>");
        }


    }
}
