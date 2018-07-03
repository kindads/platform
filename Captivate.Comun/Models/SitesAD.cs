using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Models
{
    public class SitesAD
    {
        public List<SiteAD> value { set; get; }

        public SitesAD()
        {
            value = new List<SiteAD>();
        }
    }
}
