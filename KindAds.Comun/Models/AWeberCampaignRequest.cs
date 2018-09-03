using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class AWeberCampaignRequest
    {
        public string Name { set; get; }

        public string Text { set; get; }

        public string AppKey { set; get; }

        public string AppSecret { set; get; }

        public string   OAuthToken { set; get; }

        public string OAuthSecret { set; get; }

        public string OAuthVerify { set; get; }

        public string AList { set; get; }

        public string BodyHTML { set; get; }

        public string Subject { set; get; }
    }
}
