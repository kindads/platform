using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class MailChimpCampaignRequest
    {
        public string Name { set; get; } // campaign

        public string Text { set; get; } // campaign

        public string ListId { set; get; }

        public string Subject { set; get; }

        public string FromEmail { set; get; }

        public string FromName { set; get; }

        public string ApiKey { set; get; }

        public string TemplateId { set; get; }
    }
}
