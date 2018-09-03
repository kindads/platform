using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class ConversationInvitationDocument: KindAdsV2Document
    {
        public string ConversationId { set; get; }

        public string UserIdToInvite { set; get; }

        public bool Status { set; get; }

        public string Message { set; get; }

        public ConversationInvitationDocument()
        {
            ConversationId = string.Format("<NULL>");
            UserIdToInvite = string.Format("<NULL>");
            Status = false;
            Message = string.Format("<NULL>");
        }
    }
}
