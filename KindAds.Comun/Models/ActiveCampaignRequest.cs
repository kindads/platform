using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class ActiveCampaignRequest
    {
        public string Name { set; get; }

        public string Text { set; get; }

        public string ApiKey { set; get; }

        public string Url { set; get; }

        public string FromEmail { set; get; }

        public string FromName { set; get; }

        public string Subject { set; get; }

        public string ListId { set; get; }
    }
}
