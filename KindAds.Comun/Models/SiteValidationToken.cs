using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Models
{
    public class SiteValidationToken
    {
        public string Token { set; get; }

        public string AudienceId { set; get; }

        public string SiteUrl { set; get; }

        public string Ip { set; get; }

        public string UserAgent { set; get; }

        public string RequestUrl { set; get; }
    }
   
}
