using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.KindAdsV2
{
    public class ConversationForm
    {
        public string Name { set; get; }

        List<UserDocument> People { set; get; }

        public string Message { set; get; }

        public string ChatMessage { set; get; }

        public string PeopleRaw { set; get; }

        public ConversationForm()
        {
            Name = string.Empty;
            People = new List<UserDocument>();
            Message = string.Empty;
            PeopleRaw = string.Empty;
            ChatMessage = string.Empty;
        }
    }
}
