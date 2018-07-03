using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Models
{
    public class PropertieSiteAD
    {
        public string name { set; get; }

        public string state { set; get; }

        public List<string> hostNames { set; get; }

        public PropertieSiteAD()
        {
            name = string.Empty;
            state = string.Empty;
            hostNames = new List<string>();
        }
    }
}
