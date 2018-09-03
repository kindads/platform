using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class SendGridCampaignRequest
    {
        public string ApiKey { set; get; } // ya

        public string Text { set; get; } // campaign.AdText

        public string ListId { set; get; } // ya

        public string SenderId { set; get; } // ya

        public string Subject { set; get; } // ya

        public string UnsubscriberGroupId { set; get; }

        public string Name { set; get; } // campaign.Name
    }
}
