using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.KindAdsV2
{
    public class ValidateChannelRequest
    {
        public string ApiToken { set; get; }
        public string ChannelId { set; get; }
        public string ProviderId { set; get; }
        public string SecretKey { set; get; }

        public ValidateChannelRequest()
        {
            ApiToken = string.Empty;
            ChannelId = string.Empty;
            ProviderId = string.Empty;
            SecretKey = string.Empty;
        }
    }
}
