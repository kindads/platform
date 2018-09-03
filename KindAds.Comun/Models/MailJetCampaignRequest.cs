using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class MailJetCampaignRequest
    {
        public string ApiKey { set; get; }

        public string SecretKey { set; get; }

        public string Subject { set; get; }

        public string ListId { set; get; }

        public string Name { set; get; } // campaign

        public string SegmentId { set; get; }

        public string Text { set; get; } // campaign
    }
}
