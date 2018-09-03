using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class GetResponseCampaignRequest
    {
        public string Name { set; get; }

        public string Value { set; get; }

        public string Subject { set; get; }

        public string FromFiledId { set; get; }

        public string ListId { set; get; }

        public string Text { set; get; }

        public string ApiKey { set; get; }
    }
}
