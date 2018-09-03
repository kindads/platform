using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class ProposalAnswerDocument : KindAdsV2Document
    {
        public string QuestionAskToAudienceChannelId { set; get; }
        public string Question { set; get; }
        public string Answer { set; get; }
        

        public ProposalAnswerDocument()
        {
            Question = string.Format("<NULL>");
            Answer = string.Format("<NULL>");
            QuestionAskToAudienceChannelId = string.Format("<NULL>");
        }
    }
}
