using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Models
{
    public class SiteAD
    {
        public string id { set; get; }

        public string name { set; get; }

        public string type { set; get; }

        public string kind { set; get; }

        public string location { set; get; }

        public PropertieSiteAD properties { set; get; }

        public SiteAD()
        {
            id = string.Empty;
            name = string.Empty;
            type = string.Empty;
            kind = string.Empty;
            location = string.Empty;
            properties = new PropertieSiteAD();
        }
    }
}
