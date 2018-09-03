using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class ConversationDocument : KindAdsV2Document
    {
        public string Name { set; get; }

        public string AudienceChannelId { set; get; }

        public ConversationDocument()
        {
            Name = string.Format("<NULL>");
        }
    }
}
