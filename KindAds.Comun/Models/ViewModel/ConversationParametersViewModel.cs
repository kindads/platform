using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.ViewModel
{
    public class ConversationParametersViewModel
    {
        public string PublisherId { set; get; }
        public string AdvertiserId { set; get; }
        public string NameConversation { set; get; }
        public string AudienceChannelId { set; get; }

        public ConversationParametersViewModel()
        {
            PublisherId = string.Empty;
            AdvertiserId = string.Empty;
            NameConversation = string.Empty;
            AudienceChannelId = string.Empty;
        }
    }
}
