using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class QuestionAskToAudienceChannelDocument : KindAdsV2Document
    {
        public string AudienceChannelId { set; get; }
        public string Question { set; get; }


        public QuestionAskToAudienceChannelDocument(string question)
        {
            Id = Guid.NewGuid().ToString();
            Question = question;
            
        }

        public QuestionAskToAudienceChannelDocument()
        {
            AudienceChannelId = string.Format("<NULL>");
            Question = string.Format("<NULL>");
            
        }


    }
}
