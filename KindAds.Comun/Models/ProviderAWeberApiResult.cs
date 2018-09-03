using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models
{
    public class ProviderAWeberApiResult
    {
        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }
        public string ApplicationKey { get; set; }
        public string ApplicationSecret { get; set; }
        public string RequestToken { get; set; }
        public string TokenSecret { get; set; }
        public string OauthVerifier { get; set; }
        public string CallbackURL { get; set; }

        public bool Success { get; set; }
    }
}
