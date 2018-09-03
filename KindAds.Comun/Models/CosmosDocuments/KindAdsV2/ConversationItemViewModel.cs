using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.CosmosDocuments.KindAdsV2
{
    public class ConversationItemViewModel
    {
        public string ProfileName { set; get; }
        public string ProfileTagLine { set; get; }
        public string IconUrl { set; get; }
        public string ConversationId { set; get; }

        public ConversationMessageDocument LastMessage { set; get; }

        public List<ConversationMessageDocument> Messages { set; get; }

        public List<ChunkMessage> Chunks { set; get; }

        public ConversationItemViewModel()
        {
            ProfileName = string.Empty;
            ProfileTagLine = string.Empty;
            IconUrl = string.Empty;
            ConversationId = string.Empty;

            LastMessage = new ConversationMessageDocument();
            Messages = new List<ConversationMessageDocument>();
            Chunks = new List<ChunkMessage>();
        }
    }
}
