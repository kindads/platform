using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class ConversationPeopleDocument : KindAdsV2Document
    {
        public string ConversationId { set; get; }

        public string ProfileId { set; get; }

        public ConversationPeopleDocument()
        {
            ConversationId = string.Format("<NULL>");
            ProfileId = string.Format("<NULL>");
        }
    }
}
