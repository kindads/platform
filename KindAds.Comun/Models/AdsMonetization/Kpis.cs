using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.AdsMonetization
{
    public  class Kpis
    {
        public List<string> userIds { set; get; }
        public List<string> siteIds { set; get; }
        public List<string> ips  { set; get; }
        public List<string> types { set; get; }
        public List<string> metrics { set; get; }

        public Kpis()
        {
            userIds = new List<string>();
            siteIds = new List<string>();
            ips = new List<string>();
            types = new List<string>();
            metrics = new List<string>();
        }
    }
}
