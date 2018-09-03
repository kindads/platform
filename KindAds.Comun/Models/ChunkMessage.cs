using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class ChunkMessage
    {
        public string ElementToAttach { set; get; }

        public string HeadingDate { set; get; }

        public List<ConversationMessageDocument> Messages { set; get; }

        public DateTime DateConversation { set; get; }

        public ChunkMessage()
        {
            ElementToAttach = string.Empty;
            Messages = new List<ConversationMessageDocument>();
        }
    }
}
