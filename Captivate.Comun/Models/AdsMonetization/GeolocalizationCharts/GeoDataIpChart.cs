using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models
{
    public class GeoDataIpChart
    {
        List<GeoDataIp> timeline { set; get; }

        public GeoDataIpChart()
        {
            timeline = new List<GeoDataIp>();
        }
    }

    public class GeoDataIp
    {
        public string request { set; get; } // (int)->(string)

        public string hour { set; get; } // (int) ->(string)
    }
}
